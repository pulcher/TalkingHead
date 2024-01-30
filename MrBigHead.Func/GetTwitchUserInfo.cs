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

namespace MrBigHead.Func
{
    public class GetTwitchUserInfo
    {
        private readonly ILogger _logger;

        public GetTwitchUserInfo(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetTwitchUserInfo>();
        }

        [Function("GetTwitchUserInfo")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var keyVaultName = "mbh-keyvault";
            var secretName = "TwitchAPI-ClientId";

            _logger.LogInformation("about to make a SecretClient");
            var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net"), new DefaultAzureCredential());
            _logger.LogInformation("made a SecretClient");

            KeyVaultSecret secret;

            try
            {
                secret = secretClient.GetSecret(secretName);
                _logger.LogInformation("Got a secret");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw and error on GetSecret: {ex.Message}");
                throw;
            }
;
            _logger.LogInformation("is the secret null");

            TwitchUserInformation? twitchResponse;

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", req.Query["accessToken"]);
                client.DefaultRequestHeaders.Add("Client-Id", secret.Value);

                twitchResponse = await client.GetFromJsonAsync<TwitchUserInformation>("https://api.twitch.tv/helix/users");

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
                DisplayName = twitchResponse?.Display_name,
                Email = twitchResponse?.Email,
                ImageUrl = twitchResponse?.Profile_image_url,
            };

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(userinfo);

            return response;
        }
    }
}
