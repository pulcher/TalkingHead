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
    public class VoiceService : IVoiceService
    {
        private List<Saying> sayings;
        private ILogger logger;
        private HttpClient client;
        private readonly IHttpClientFactory httpClientFactory;

        public VoiceService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            this.sayings = new List<Saying>();
            this.logger = loggerFactory.CreateLogger("Generic Logger");

            this.client = httpClientFactory.CreateClient();

            // do this until the get work from the functions
            //this.voices = GetDefaultVoice();
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IList<Voice>> GetAllVoicesAsync()
        {
            var voices = await client.GetFromJsonAsync<IList<Voice>>("https://bigheadfuncs.azurewebsites.net/api/getallvoices");

            return voices;
        }
    }
}
