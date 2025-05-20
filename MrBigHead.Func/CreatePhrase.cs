using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MrBigHead.Func
{
    public class CreatePhrase
    {
        private readonly ILogger<CreatePhrase> _logger;

        public CreatePhrase(ILogger<CreatePhrase> logger)
        {
            _logger = logger;
        }

        [Function(nameof(CreatePhrase))]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to .NET isolated worker !!");

            return response;
        }
    }
}
