using System.ComponentModel.DataAnnotations;
using System.Net;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
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

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            _logger.LogInformation("is the secret null");
            if (secret != null)
            {
                response.WriteString("secrets are not blank");
            }
            else
            {
                _logger.LogInformation("yep");
                response.WriteString("Welcome to Azure Functions!");
            };

            return response;
        }
}
}
