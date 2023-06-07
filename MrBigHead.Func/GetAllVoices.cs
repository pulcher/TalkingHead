using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Net;

namespace MrBigHead.Func
{
    public class GetAllVoices
    {
        private readonly ILogger _logger;

        public GetAllVoices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetAllVoices>();
        }

        [Function(nameof(GetAllVoices))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestData req,
            [TableInput("Voices")] IEnumerable<VoiceEntity> entities)
        {
            _logger.LogInformation("triggered: GetAllVoices()");

            var voices = new List<Voice>();

            foreach (var entity in entities)
            {
                _logger.LogInformation($"entity: {entity.Category}:{entity.Name}:{entity.Language}");
                voices.Add(new Voice { Name = entity.Name, Language = entity.Name, IsDefault = entity.IsDefault });
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(voices);

            return response;
        }
    }
}
