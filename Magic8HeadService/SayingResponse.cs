using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrBigHead.Services;
using MrBigHead.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magic8HeadService
{
    public class SayingResponse : ISayingResponse
    {
        private Random random;
        private IConfiguration config;
        private readonly ISayingService sayingsService;
        private readonly ILogger<Worker> logger;
        private IList<Saying> sayings;
        private readonly SpeechSynthesizer speechSynthesizer;

        public SayingResponse(IConfiguration config, ISayingService sayingsService, ILogger<Worker> logger)
        {
            this.config = config;
            this.sayingsService = sayingsService;
            this.logger = logger;

            var speechSubscription = config["TwitchBotConfiguration:SpeechSubscription"];

            var speechServiceRegion = config["TwitchBotConfiguration:SpeechServiceRegion"];
            // create a new speech synth
            var speechConfig = SpeechConfig
                .FromSubscription(config["TwitchBotConfiguration:SpeechSubscription"], config["TwitchBotConfiguration:SpeechServiceRegion"]);
            speechConfig.SpeechSynthesisLanguage = "en-GB";
            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            logger.LogInformation($"Here is the SpeechSynthesisLanguage: {speechConfig.SpeechSynthesisLanguage}");
            logger.LogInformation($"Here is the SpeechSynthesisVoiceName: {speechConfig.SpeechSynthesisVoiceName}");

            speechSynthesizer = new SpeechSynthesizer(speechConfig);

            SetupSayingsAsync().Wait();
        }

        public async Task SetupSayingsAsync()
        {
            logger.LogInformation("Setiing up Sayings...");

            random = new Random();
            sayings = await sayingsService.GetAllSayingsAsync();

            logger.LogInformation($"saying count: {sayings.Count}");
        }

        public async Task SaySomethingNice(string message)
        {
            logger.LogInformation($"Saying: {message}");

            using (var result = await speechSynthesizer.SpeakTextAsync(message))
            {
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    logger.LogInformation($"Speech synthesized to speaker for text [{message}]");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    logger.LogInformation($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        logger.LogError($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        logger.LogError($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        logger.LogError($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
            return;
        }

        public string PickSaying()
        {
            return PickSaying(Moods.Snarky);
        }

        public string PickSaying(string mood)
        {
            logger.LogInformation("PickSaying: ");

            var currentSelectedSayings = sayings.Select(p => p)
                .Where(m => m.Mood == mood)
                .ToArray();

            var index = random.Next(currentSelectedSayings.Length);

            logger.LogInformation($"PickSaying: count: {currentSelectedSayings.Length}, random: {index}");
            var pickedSaying = currentSelectedSayings[index];

            return pickedSaying.Phrase;
        }
    }
}