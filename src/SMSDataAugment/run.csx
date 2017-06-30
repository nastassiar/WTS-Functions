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
    
    // dt_send is never null
    DateTime messageTime = msg.dt_send.Value;

    // Get all the currently valid campaigns
    IList<Campaign> currentCampaigns = campaignTable.Where(i => i.StartTime < messageTime && i.EndTime > messageTime).ToList<Campaign>();
    IList<string> campaigns = new List<string>();
    foreach(Campaign c in currentCampaigns)
    {
        foreach(string t in c.Terms)
        {
            if (msg.message != null && msg.message.Contains(t))
            {
                campaigns.Add(c.RowKey);
            }
        }
    }

    msg.campaigns = campaigns;

    string json = JsonConvert.SerializeObject(msg);

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
    public IList<string> Terms { get; set; }
    public IList<string> PostIds { get; set; }

}