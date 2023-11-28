using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Magic8HeadService
{
    public class DadJokeService : IDadJokeService
    {
        private readonly ISayingResponse sayingResponse;
        private ILogger logger;

        public DadJokeService(ISayingResponse sayingResponse, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger("Generic Logger");
            this.sayingResponse = sayingResponse;
        }

        public string GetDadJoke()
        {
            return GetRandomAnswer();
        }

        private string GetRandomAnswer()
        {
            return sayingResponse.PickSaying("dad");
        }
    }
}
