using System.ComponentModel.DataAnnotations;
using System.Net;
using Azure.Core;
using System.Net.Http.Headers;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MrBigHead.Shared;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

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
                    _logger.LogError($"Threw and error on GetSecret: {ex.Message}");
                    throw;
                }
            };

            _logger.LogInformation("is the secret null");

            TwitchUserInformation? twitchResponse = new();

            using (HttpClient client = new())
            {
                var testToken = req.Query["accessToken"];

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", req.Query["accessToken"]);
                client.DefaultRequestHeaders.Add("Client-Id", clientId);


                try
                {
                    var responseString = await client.GetStringAsync("https://api.twitch.tv/helix/users");
                    var something = JsonObject.Parse(responseString);
                    var somethingelse = something["data"];

                    twitchResponse = somethingelse[0].Deserialize<TwitchUserInformation>();


                    var test = string.Empty;
                    //string test = await client.GetStringAsync("https://api.twitch.tv/helix/users");
                }
                catch (Exception ex)
                {
                    var test = ex.Message;
                };

                //if (response.IsSuccessStatusCode)
                //{
                //    string apiResponse = await response.Content.ReadAsStringAsync();
                //    return new OkObjectResult(apiResponse);
                //}
                //else
                //{
                //    return new BadRequestObjectResult("Failed to call the API");
                //}
            }

            var userinfo = new UserInformation
            {
                UserName = twitchResponse?.Login,
                DisplayName = twitchResponse?.DisplayName,
                Email = twitchResponse?.Email,
                ImageUrl = twitchResponse?.ProfileImageUrl,
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(userinfo);

            return response;
        }
    }
}
