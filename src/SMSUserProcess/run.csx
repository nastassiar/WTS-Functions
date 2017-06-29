#r "Newtonsoft.Json"

 #load "../Common.csx"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public static void Run(string queueItem, ICollector<object> output, TraceWriter log)
{
    dynamic m = JObject.Parse(queueItem);
    var i = m.client;
    string location = m.location;

    var surveys = i.client_surveys;

    // Need to do this since the API doesn't return a list!
    JObject attributesAsJObject = i.client_surveys;
    Dictionary<string, dynamic> values = attributesAsJObject.ToObject<Dictionary<string, object>>();
    foreach(KeyValuePair<string, dynamic> entry in values)
    {
        dynamic e = entry.Value;
        IEnumerable<dynamic> responses = e.responses;
        // From Responses
        string name =  responses.FirstOrDefault(p => p.qid == "NAME")?.parsed_response; //"qid":"NAME"
        string yob = responses.FirstOrDefault(p => p.qid == "Y.O.B")?.parsed_response ?? responses.FirstOrDefault(p => p.qid == "YOB")?.parsed_response;// "qid":"Y.O.B"
        string gender = responses.FirstOrDefault(p => p.qid == "GENDER")?.parsed_response; // "qid":"GENDER"
        string county = responses.FirstOrDefault(p => p.qid == "COUNTY")?.parsed_response;; // "qid":"COUNTY"

        var obj = new {
            source = "SMS",
            startTimestamp = e.start_timestamp != null ? ConvertTimestampToDatetime(e.start_timestamp.Value, log) : DateTime.Now,
            completeTimestamp = e.complete_timestamp ? ConvertTimestampToDatetime(e.complete_timestamp.Value, log) : null,
            phoneNumber = i.phone,
            lat = i.lat,
            lon = i.lon,
            gender = gender == "1" ? "Female" : "Male",
            yob = yob,
            name = name ?? i.name, 
            county = county ?? i.locationTextRaw,
            country = location
        };
        
        log.Info("OBJ : "+obj);
        output.Add(obj);
    }
}
