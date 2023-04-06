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
        private Random random = new();
        private TwitchBotConfiguration twitchBotConfiguration;
        private readonly ISayingService sayingsService;
        private readonly ILogger<Worker> logger;
        private IList<Saying> sayings;
        private IList<SpeechConfig> speechConfigs;
        private readonly SpeechSynthesizer speechSynthesizer;
        private readonly Dictionary<string, SpeechSynthesizer> speechSynthesizers = new Dictionary<string, SpeechSynthesizer>();

        public SayingResponse(TwitchBotConfiguration twitchBotConfiguration, ISayingService sayingsService, ILogger<Worker> logger)
        {
            this.twitchBotConfiguration = twitchBotConfiguration;
            this.sayingsService = sayingsService;
            this.logger = logger;

            // create a new speech synth
            var speechConfig = SpeechConfig
                .FromSubscription(twitchBotConfiguration.SpeechSubscription, 
                twitchBotConfiguration.SpeechServiceRegion);

            SetupVoiceListAsync().Wait();
            SetupSpeechSynthizersAsync().Wait();

            speechSynthesizer = new SpeechSynthesizer(speechConfig);

            SetupSayingsAsync().Wait();
        }

        public async Task SetupSpeechSynthizersAsync()
        {
            foreach (var voice in speechConfigs)
            {
                if (!speechSynthesizers.ContainsKey(voice.SpeechSynthesisVoiceName))
                {
                    speechSynthesizers.Add(voice.SpeechSynthesisVoiceName, new SpeechSynthesizer(voice));
                }
            }
        }

        public async Task SetupVoiceListAsync()
        {
            speechConfigs = new List<SpeechConfig>();

            foreach (var voice in GetVoices())
            {
                // create a new speech synth
                var speechConfig = SpeechConfig
                    .FromSubscription(twitchBotConfiguration.SpeechSubscription,
                    twitchBotConfiguration.SpeechServiceRegion);

                speechConfig.SpeechSynthesisLanguage = voice.Language;
                speechConfig.SpeechSynthesisVoiceName = voice.Name;

                speechConfigs.Add(speechConfig);
            }

            //speechConfig = SpeechConfig
            //    .FromSubscription(twitchBotConfiguration.SpeechSubscription,
            //    twitchBotConfiguration.SpeechServiceRegion);

            //speechConfig.SpeechSynthesisLanguage = "en-US";
            //speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

            //speechConfigs.Add(speechConfig);
        }

        private IEnumerable<voice> GetVoices()
        {
            return new List<voice>
            {
                new voice {Language = "en-GB", Name = "en-GB-RyanNeural"},
                new voice {Language = "en-US", Name = "ru-RU-SvetlanaNeural"},
                new voice {Language = "en-PH", Name = "en-PH-RosaNeural"},
                new voice {Language = "en-US", Name = "en-US-JennyNeural"},
                new voice {Language = "en-US", Name = "cy-GB-AledNeural"},
                new voice {Language = "en-US", Name = "fr-CA-SylvieNeural"},
                new voice {Language = "en-US", Name = "fr-CA-JeanNeural"},
                new voice {Language = "en-US", Name = "fil-PH-AngeloNeural"},
                new voice {Language = "en-US", Name = "kk-KZ-DauletNeural"},
                new voice {Language = "en-US", Name = "sl-SI-PetraNeural"}
            };
        }

        public async Task SetupSayingsAsync()
        {
            logger.LogInformation("Setiing up Sayings...");

            sayings = await sayingsService.GetAllSayingsAsync();

            logger.LogInformation($"saying count: {sayings.Count}");
        }

        public async Task SaySomethingNice(string message)
        {
            logger.LogInformation($"Saying: {message}");

            var speechSynthesizer = GetSpeechSynthizer();

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

        private SpeechSynthesizer GetSpeechSynthizer()
        {
            var index = random.Next(speechConfigs.Count);

            logger.LogInformation("Language in use: {language}", speechConfigs[index].SpeechSynthesisLanguage);
            logger.LogInformation("Voice in use: {voice}", speechConfigs[index].SpeechSynthesisVoiceName);

            return speechSynthesizers[speechConfigs[index].SpeechSynthesisVoiceName];
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

            if (currentSelectedSayings.Length == 0)
                return string.Empty;

            var index = random.Next(currentSelectedSayings.Length);

            logger.LogInformation($"PickSaying: count: {currentSelectedSayings.Length}, random: {index}");
            var pickedSaying = currentSelectedSayings[index];

            return pickedSaying.Phrase;
        }
    }

    internal class voice
    {
        public string Language { get; set; }
        public string Name { get; set; }
    }
}
