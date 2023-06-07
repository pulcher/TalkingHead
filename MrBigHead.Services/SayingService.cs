using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MrBigHead.Services
{
    public class SayingService : ISayingService
    {
        private List<Saying> sayings;
        private ILogger logger;
        private HttpClient client;
        private readonly IHttpClientFactory httpClientFactory;

        public SayingService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            this.sayings = new List<Saying>();
            this.logger = loggerFactory.CreateLogger("Generic Logger");

            this.client = httpClientFactory.CreateClient();

            // do this until the get work from the functions
            //this.sayings = GetDefaultSaying();
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IList<Saying>> GetAllSayingsAsync()
        {
            var sayings = await client.GetFromJsonAsync<IList<Saying>>("https://bigheadfuncs.azurewebsites.net/api/GetAllPhrases");

            return sayings;
        }
    }
}
