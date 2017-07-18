To Deploy JUST these functions to Azure Portal

- Go to the portal and login
- Click add and select function app
- Fill in all necessary fields and click create
- After deployment has finished go to Platform Features
- Click on Deployment options 
- Add you github credentials and link to this repository
- If the functions don't automatically create go to Deployment Options and click sync
- The following functions should be created:
  - FBDataProcess : Function for formatting FB data into correct types. Also passes user data from posts to separate process for updating users.
  - FBDataStore : Function that stores the formatted FB data into documentDB.
  - FBUserStore : Function that updates users based on the data recieved from the facebook page activity.
  - SMSDataStore : Function that stores the sms data.
  - SMSDataAugment : Function that adds the campaign Ids to the SMS Data.
  - SMSSUserProcess: Function that parses out the user data from the SMS userlogs messages.
  - SMSUserStore : Function that updates or creates users based on the SMS data.
  - CampaignAPI : API Endpoints for creating and updating campaigns
 
- After the functions have been created go to Platform Features
- Select Application Setting
- Add an app settings for:
  - 'AzureWebJobsServiceBus' with the connection string to the service bus queue
  - 'CosmosDB_Connection' with the connection string to CosmosDB
  - 'FB_Verify_Token' with the verify token set in the Facebook developer portal
  - 'WTSStorage_Connection' witht the connection string for the Storage account where the Capaigns table is 


These steps will deploy the functions but it will still be necessary to 
1. Set up the SMS and Facebook webhooks! 
2. Set up the Table Storage and CosmosDB
- Table storage is a storage account with a table called campaigns in it
- CosmosDB is either a docdb or mongodb database with a collection for FBData, SMSData and Users
2. Set up all of the following service bus queues
- fb_data_toprocess
- fb_data_tostore
- fb_user_tostore
- sms_data_toprocess
- sms_data_toaugment
- sms_data_tostore
- sms_user_toprocess
- sms_user_tostore


