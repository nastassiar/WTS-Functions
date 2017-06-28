#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using System;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Campaign> entries, ICollector<Campaign> tableBinding, TraceWriter log)
{
    // Endpoints for  creating and updating campaigns!

    // Id needed for updates and some gets!
    string id = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
        .Value;
    log.Info("ID : "+ id);

    if (req.Method == HttpMethod.Get)
    {
        if (id == null)
        {
            IList<Campaign> campaigns = entries.ToList();

            log.Info("Getting all campaigns!");
            log.Info("campaign count :"+ campaigns.Count);
            log.Info("campaigns : "+ campaigns);
            
            var json = JsonConvert.SerializeObject(campaigns);
            return req.CreateResponse(HttpStatusCode.OK, json);
        }
        else 
        {
            Campaign existingCampaign = entries.Where(i => i.RowKey == id).FirstOrDefault();

            log.Info("Getting a campaign!");
            return existingCampaign == null
                ? req.CreateResponse(HttpStatusCode.NotFound, $"Campaign with id : {id} not found")
                : req.CreateResponse(HttpStatusCode.OK, existingCampaign);
                }
    }
    else if (req.Method == HttpMethod.Post)
    {
        Campaign requestCampaign = await req.Content.ReadAsAsync<Campaign>();

        // Used to modify and update a resource
        if (id != null)
        {
            Campaign existingCampaign =  entries.Where(i => i.RowKey == id).FirstOrDefault();
            // Updating an existing object!
            if (existingCampaign == null)
            {
                // No object to update! Return an error!
                return req.CreateResponse(HttpStatusCode.NotFound, $"Campaign with id : {id} not found");
            }
            else 
            {
                existingCampaign.Name = requestCampaign.Name ?? existingCampaign.Name;
                existingCampaign.Description = requestCampaign.Description ?? existingCampaign.Description;
                existingCampaign.StartTime = requestCampaign.StartTime ?? existingCampaign.StartTime;
                existingCampaign.EndTime = requestCampaign.EndTime ?? existingCampaign.EndTime;
                existingCampaign.LastUpdatedTime = DateTime.Now;
                existingCampaign.Terms = existingCampaign.Terms.Union(requestCampaign.Terms).ToList();
                existingCampaign.PostIds = existingCampaign.PostIds.Union(requestCampaign.PostIds).ToList();
                
                tableBinding.Add(existingCampaign);
                return req.CreateResponse(HttpStatusCode.OK, existingCampaign);
            }
        }
        else 
        {
            // create a new object!
            Campaign newCampaign = new Campaign()
            {
                PartitionKey = "ShujaazCampaigns",
                // Row Key is based on time
                RowKey = DateTime.UtcNow.Ticks.ToString("d20"),
                Name = requestCampaign.Name,
                Description = requestCampaign.Description,
                StartTime = requestCampaign.StartTime ?? DateTime.Now,
                EndTime = requestCampaign.EndTime ?? DateTime.Now.AddYears(1),
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now,
                Terms = requestCampaign.Terms ?? new List<string>(),
                PostIds =  requestCampaign.PostIds ?? new List<string>()
            };

             tableBinding.Add(newCampaign);
             return req.CreateResponse(HttpStatusCode.OK, newCampaign);
        }
    }
    else if (req.Method == HttpMethod.Put)
    {
         // Used to create or overwrite a resource
         Campaign requestCampaign = await req.Content.ReadAsAsync<Campaign>();

        // Used to modify and update a resource
        if (id != null)
        {
            Campaign existingCampaign =  entries.Where(i => i.RowKey == id).FirstOrDefault();
            if (existingCampaign == null)
            {
                
                // create a new object!
                Campaign newCampaign = new Campaign()
                {
                    PartitionKey = "ShujaazCampaigns",
                    // Row Key is based on time
                    RowKey = id,
                    Name = requestCampaign.Name,
                    Description = requestCampaign.Description,
                    StartTime = requestCampaign.StartTime ?? DateTime.Now,
                    EndTime = requestCampaign.EndTime ?? DateTime.Now.AddYears(1),
                    CreatedTime = DateTime.Now,
                    LastUpdatedTime = DateTime.Now,
                    Terms = requestCampaign.Terms ?? new List<string>(),
                    PostIds = requestCampaign.PostIds ?? new List<string>()
                };

                tableBinding.Add(newCampaign);
                
                return req.CreateResponse(HttpStatusCode.OK, newCampaign);
            }
            else 
            {
                existingCampaign.Name = requestCampaign.Name ?? existingCampaign.Name;
                existingCampaign.Description = requestCampaign.Description ?? existingCampaign.Description;
                existingCampaign.StartTime = requestCampaign.StartTime ?? existingCampaign.StartTime;
                existingCampaign.EndTime = requestCampaign.EndTime ?? existingCampaign.EndTime;
                existingCampaign.LastUpdatedTime = DateTime.Now;
                existingCampaign.Terms = requestCampaign.Terms ?? existingCampaign.Terms;
                existingCampaign.PostIds = requestCampaign.PostIds ?? existingCampaign.PostIds;
                
                tableBinding.Add(existingCampaign);
                return req.CreateResponse(HttpStatusCode.OK, existingCampaign);
            }
        }
        else 
        {
            return req.CreateResponse(HttpStatusCode.BadRequest, "Id is required for PUT to overwrite or create a campaign");
        }
    }
    else
    {
         return req.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request Method");
    }
}

public class Campaign : TableEntity
{
    //public string PartitionKey { get; set; }
    //public string RowKey { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime? StartTime  { get; set; }
    public DateTime? EndTime  { get; set; }
    public DateTime? CreatedTime  { get; set; }
    public DateTime? LastUpdatedTime  { get; set; }
    public IList<string> Terms { get; set; }
    public IList<string> PostIds { get; set; }

}