# Possible func commands that need remembering

  16 func new --name GetAllQuips --template "HTTP trigger" --authlevel "anonymous"
  17 dir
  18 more .\GetAllQuips.cs
  19 ls
  20 func start
  21 az login
  22 az group list
  23 az storage account create --name mr-big-head-storage --location southcentralus --resource-group mr-big-head-bot...
  24 az storage account create --name mrbigheadstorage --location southcentralus --resource-group mr-big-head-bot --...
  25 az functionapp create --resource-group mr-big-head-bot --consumption-plan-location southcentralus --runtime dot...
  26 func azure functionapp publish bigheadfuncs
  27 func azure functionapp logstream bigheadfuncs

# Tesing with the twitch cli

Verification messages.  aka. challlenges
```
twitch event verify-subscription subscribe -F  http://localhost:7071/api/WebhookEndpoint -s 5f1a6e7cd2e7137ccf9e15b2f43fe63949eb84b1db83c1d5a867dc93429de4e4
```

Triggering events
```
twitch event trigger subscribe -F http://localhost:7071/api/WebhookEndpoint -s 5f1a6e7cd2e7137ccf9e15b2f43fe63949eb84b1db83c1d5a867dc93429de4e4
```

List of all the possible events: https://dev.twitch.tv/docs/cli/event-command/#triggering-webhook-events

# links and stuff

https://learn.microsoft.com/en-us/azure/azure-web-pubsub/quickstart-serverless?tabs=csharp

```
dotnet add package Microsoft.Azure.WebJobs.Extensions.WebPubSub
```

https://www.serverless360.com/blog/remote-debugging-azure-functions-in-visual-studio-2022#:~:text=How%20to%20remote%20debug%20an%20HTTP%20trigger%20Azure,message%20to%20your%20HTTP%20trigger%20Azure%20Function%20

https://azurelessons.com/how-to-access-app-setting-azure-functions/


