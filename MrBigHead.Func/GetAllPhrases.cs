using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Net;

namespace MrBigHead.Func
{
    public class GetAllPhrases
    {
        private readonly ILogger _logger;

        public GetAllPhrases(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetAllPhrases>();
        }

        [Function(nameof(GetAllPhrases))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            [TableInput("Sayings")] IEnumerable<SayingEntity> entities)
        {
            _logger.LogInformation("GetAllPhrases: get called");

            var sayings = new List<Saying>();

            foreach (var entity in entities)
            {
                _logger.LogInformation($"entity: {entity.Mood}:{entity.Phrase}");
                sayings.Add(new Saying { Mood = entity.Mood, Phrase = entity.Phrase });
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(sayings);

            return response;
        }
    }
}
