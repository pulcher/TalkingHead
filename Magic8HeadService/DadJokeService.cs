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

        public async Task<DadJoke> GetDadJoke(string url)
        {
            var siteUrl = "https://karljoke.herokuapp.com/jokes/random";

            if (!string.IsNullOrEmpty(url))
                siteUrl = url;
                
            //https://dadjoke-2021-1018a.jimf99.repl.co/jokes/random

            var dadJoke = await client.GetFromJsonAsync<DadJoke>(siteUrl);

            dadJoke = await client.GetFromJsonAsync<DadJoke>("https://karljoke.herokuapp.com/jokes/random");

            return dadJoke;
        }
    }
}
