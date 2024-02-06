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
        private string errorMessage;

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

            var clientId= configuration[secretName];

            _logger.LogInformation("about to make a SecretClient");
            var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net"), new DefaultAzureCredential());
            _logger.LogInformation("made a SecretClient");

            KeyVaultSecret secret;

            if (clientId == null)
            {
                try
                {
                    secret = secretClient.GetSecret(secretName);
                    _logger.LogInformation("Got a secret");
                    clientId = secret.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Threw and error on GetSecret");
                    throw;
                }
            };

            TwitchUserInformation? twitchResponse = new();

            using (HttpClient client = new())
            {
                var token = req.Query["accessToken"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("Client-Id", clientId);

                try
                {
                    var responseString = await client.GetStringAsync("https://api.twitch.tv/helix/users");
                    var arrayOfTwitchData = JsonNode.Parse(responseString);
                    var firstUser = arrayOfTwitchData["data"];

                    twitchResponse = firstUser[0].Deserialize<TwitchUserInformation>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Threw and error on GetStringAsync");
                    errorMessage = ex.Message;
                };
            }

            var userinfo = new UserInformation
            {
                UserName = twitchResponse?.Login,
                DisplayName = twitchResponse?.DisplayName,
                Email = twitchResponse?.Email,
                ImageUrl = twitchResponse?.ProfileImageUrl,
                ErrorMessage = errorMessage
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(userinfo);

            return response;
        }
    }
}
