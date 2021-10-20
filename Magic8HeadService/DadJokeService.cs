using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Magic8HeadService
{
    public class DadJokeService : IDadJokeService
    {
        private ILogger logger;
        private HttpClient client;
        private IHttpClientFactory httpClientFactory;

        public DadJokeService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger("Generic Logger");

            this.client = httpClientFactory.CreateClient();

            this.httpClientFactory = httpClientFactory;
        }

        public async Task<DadJoke> GetDadJoke()
        {
            var dadJoke = await client.GetFromJsonAsync<DadJoke>("https://karljoke.herokuapp.com/jokes/random");

            return dadJoke;
        }
    }
}
