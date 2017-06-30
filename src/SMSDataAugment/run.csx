#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using System;
using System.Net;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static void Run(string queueItem, IQueryable<Campaign> campaignTable, TraceWriter log, out string output)
{
    dynamic msg = JObject.Parse(queueItem);
    DateTime messageTime = msg.dt_send != null ? msg.dt_send.Value : DateTime.Now;

    IList<Campaign> currentCampaigns = campaignTable.Where(i => i.StartTime < messageTime && i.EndTime > messageTime).ToList<Campaign>();
    IList<string> campaigns = new List<string>();
   
    if (currentCampaigns != null || currentCampaigns.Count() > 0)
    {
        foreach (Campaign c in currentCampaigns)
        {
            IList<string> terms = c.Terms != null ? c.Terms.Split(',').ToList() : new List<string>(); 
            foreach(string t in terms)
            {
                if (msg.message != null && msg.message.ToLower().Contains(t.ToLower()))
                {
                    campaigns.Add(c.RowKey);
                }
            }
        }
        msg.campaigns = JArray.FromObject(campaigns);
    };
    string json = JsonConvert.SerializeObject(msg);

    log.Info("JSON:"+json);
    
    output = json;
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
    public string Terms { get; set; }
    public string PostIds { get; set; }

}