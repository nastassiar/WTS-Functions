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
    string id = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value;

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
                
                var ecTerms = existingCampaign.Terms.Split(',').ToList();
                var rcTerms = requestCampaign.Terms.Split(',').ToList();
                var combinedTerms = ecTerms.Union(rcTerms).ToList();
                
                var ecPostIds = existingCampaign.PostIds.Split(',').ToList();
                var rcPostIds = requestCampaign.PostIds.Split(',').ToList();
                var combinedPostIds = ecPostIds.Union(rcPostIds).ToList();
                
                existingCampaign.Terms = string.Join(",",combinedTerms.ToArray());
                existingCampaign.PostIds = string.Join(",",combinedPostIds.ToArray());

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
                RowKey = $"{DateTime.UtcNow.Ticks.ToString("d20")}-{requestCampaign.Name ?? "NONAME"}",
                Name = requestCampaign.Name,
                Description = requestCampaign.Description,
                StartTime = requestCampaign.StartTime ?? DateTime.Now,
                EndTime = requestCampaign.EndTime ?? DateTime.Now.AddYears(1),
                CreatedTime = DateTime.Now,
                LastUpdatedTime = DateTime.Now,
                Terms = requestCampaign.Terms,
                PostIds =  requestCampaign.PostIds
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
                    Terms = requestCampaign.Terms,
                    PostIds = requestCampaign.PostIds
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
    // THESE HAVE TO BE COMMON SEPARATED STRINGS SINCE TABLE STORAGE DOESN'T SUPPORT STRINGS :(
    public string Terms { get; set; }
    public string PostIds { get; set; }

}