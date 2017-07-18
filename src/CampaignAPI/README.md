#H1 Campaigns API
======
*Important Note:* A camapign will only start being used SMS messages after it has been created. So setting the StartTime to before the current time will have no affect on the existing Facebook or SMS messages that have already been processed. 

Endpoints for creating and updating Campaigns. 

###H3 GET campaigns/{id}
- Returns the Campaign if it exists.
- Returns :
```json
{
    "PartitionKey": "string", // Always ShujaazCampaigns
    "RowKey": "string", // Combination of timestamp when created and the name
    "Name": "string",
    "Description": "string",
    "StartTime": DateTime,
    "EndTime": DateTime,
    "CreatedTime": DateTime,
    "LastUpdatedTime" : DateTime, 
    "Terms" : "string" // A common separate list of "string"s
}
```

###H3 GET campaigns/
- Returns all the Campaigns.
    - Limited to 50 (can be changed in the function settings).
- Returns : 
```json
[
{
    "PartitionKey": "string", // Always ShujaazCampaigns
    "RowKey": "string", // Combination of timestamp when created and the name
    "Name": "string",
    "Description": "string",
    "StartTime": DateTime,
    "EndTime": DateTime,
    "CreatedTime": DateTime,
    "LastUpdatedTime" : DateTime, 
    "Terms" : "string" // A common separate list of "string"s
}
]
```

###H3 POST campaigns/
- Creates a new Campaign.
- Body : 
```json
{
    "Name": "string", // Used as part of Id so set carefully
    "Description": "string", // A description of the campaign
    "StartTime": DateTime, // When the campaign is to start, defaults to Now if not sent
    "EndTime": DateTime, // When the campaign ends, defaults to a year from now if not sent
    "Terms" : "string" // A common separate list of "string"s used to tag facebook and SMS messages witht the campaign if they contain the terms
}
- Return : 
```json
{
    "PartitionKey": "string", // Always ShujaazCampaigns
    "RowKey": "string", // Combination of timestamp when created and the name
    "Name": "string",
    "Description": "string",
    "StartTime": DateTime,
    "EndTime": DateTime,
    "CreatedTime": DateTime,
    "LastUpdatedTime" : DateTime, 
    "Terms" : "string" // A common separate list of "string"s
}
```

###H3 POST campaigns/{id}
- Update an existing campaign.
    - Will return an error if the Campaign with that Id does not exist.
- Body : 
```json
{
    "Name": "string", // Used as part of Id so set carefully
    "Description": "string", // A description of the campaign
    "StartTime": DateTime, // When the campaign is to start, defaults to Now if not sent
    "EndTime": DateTime, // When the campaign ends, defaults to a year from now if not sent
    "Terms" : "string" // Merges this with the existing list of terms
}
- Returns : 
```json
{
    "PartitionKey": "string", // Always ShujaazCampaigns
    "RowKey": "string", // Combination of timestamp when created and the name
    "Name": "string",
    "Description": "string",
    "StartTime": DateTime,
    "EndTime": DateTime,
    "CreatedTime": DateTime,
    "LastUpdatedTime" : DateTime, 
    "Terms" : "string" // A common separate list of "string"s
}
```

###H33 PUT campaigns/{id}
- Overwrite or create an existing campaign.
    - If a campaign with the ID exists it will be overwritten, otherwise it'll be created
- Body : 
```json
{
    "Name": "string", // Used as part of Id so set carefully
    "Description": "string", // A description of the campaign
    "StartTime": DateTime, // When the campaign is to start, defaults to Now if not sent
    "EndTime": DateTime, // When the campaign ends, defaults to a year from now if not sent
    "Terms" : "string" // Merges this with the existing list of terms
}
- Returns : 
```json
{
    "PartitionKey": "string", // Always ShujaazCampaigns
    "RowKey": "string", // Combination of timestamp when created and the name
    "Name": "string",
    "Description": "string",
    "StartTime": DateTime,
    "EndTime": DateTime,
    "CreatedTime": DateTime,
    "LastUpdatedTime" : DateTime, 
    "Terms" : "string" // A common separate list of "string"s
}
```

