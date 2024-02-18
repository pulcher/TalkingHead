using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MrBigHead.Func
{
    public class GetTwitchUserInfo
    {
        private readonly ILogger _logger;
        private readonly IConfiguration configuration;

        public GetTwitchUserInfo(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<GetTwitchUserInfo>();
            this.configuration = configuration;
        }

        [Function("GetTwitchUserInfo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var keyVaultName = "mbh-keyvault";
            var secretName = "TwitchAPI-ClientId";
            var secretBroadcasterName = "TwitchAPI-BroadcasterId";

            var clientId = configuration[secretName];
            var broadcasterId = configuration[secretBroadcasterName];

            var loggedInUserId = req.Query["userId"];

            _logger.LogInformation("about to make a SecretClient");
            var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net"), new DefaultAzureCredential());

            _logger.LogInformation("made a SecretClient");

            KeyVaultSecret secretCliendId;
            KeyVaultSecret secretBroadcasterId;

            if (clientId == null || broadcasterId == null)
            {
                try
                {
                    secretCliendId = secretClient.GetSecret(secretName);
                    _logger.LogInformation("Got a secret");
                    clientId = secretCliendId.Value;

                    // my broadcasterId
                    secretBroadcasterId = secretClient.GetSecret(secretName);
                    _logger.LogInformation("Got a broadcasterId");
                    broadcasterId = secretBroadcasterId.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Threw and error on GetSecret: {ex.Message}");
                    throw;
                }
            };

            _logger.LogInformation("is the secret null");

            TwitchUserInformation? twitchUserResponse = new();
            TwitchSubscriptionInformation? twitchSubscriptionResponse = new();

            using (HttpClient client = new())
            {
                var testUserId = req.Query["userId"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", req.Query["accessToken"]);
                client.DefaultRequestHeaders.Add("Client-Id", clientId);


                try
                {
                    var responseString = await client.GetStringAsync("https://api.twitch.tv/helix/users");
                    var something = JsonObject.Parse(responseString);
                    var somethingelse = something["data"];

                    twitchUserResponse = somethingelse[0].Deserialize<TwitchUserInformation>();
                }
                catch (Exception ex)
                {
                    var test = ex.Message;
                };

                try
                {
                    var responseString = await client.GetStringAsync($"https://api.twitch.tv/helix/subscriptions/user?broadcaster_id={broadcasterId}&user_id={loggedInUserId}");
                    var parsedResponse = JsonObject.Parse(responseString);
                    var responseData = parsedResponse["data"];

                    twitchSubscriptionResponse = responseData[0].Deserialize<TwitchSubscriptionInformation>();
                }
                catch (Exception ex)
                {
                    var test = ex.Message;
                }
            }

            var userinfo = new UserInformation
            {
                UserName = twitchUserResponse?.Login,
                DisplayName = twitchUserResponse?.DisplayName,
                Email = twitchUserResponse?.Email,
                ImageUrl = twitchUserResponse?.ProfileImageUrl,
                Tier = twitchSubscriptionResponse.Tier,
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(userinfo);

            return response;
        }
    }
}
