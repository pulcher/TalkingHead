using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
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

            //speechSynthesizer = new SpeechSynthesizer(speechConfig);

            speechSynthesizer = new SpeechSynthesizer(speechConfigs[0]);

            speechSynthesizer.WordBoundary += (s, e) =>
            {
                logger.LogInformation($"WordBoundary event:" +
                    // Word, Punctuation, or Sentence
                    $"\r\n\tBoundaryType: {e.BoundaryType}" +
                    $"\r\n\tAudioOffset: {(e.AudioOffset + 5000) / 10000}ms" +
                    $"\r\n\tDuration: {e.Duration}" +
                    $"\r\n\tText: \"{e.Text}\"" +
                    $"\r\n\tTextOffset: {e.TextOffset}" +
                    $"\r\n\tWordLength: {e.WordLength}");
            };

            SetupSayingsAsync().Wait();
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

                //speechConfig.SetProperty(PropertyId.SpeechServiceResponse_RequestWordBoundary, "true");

                speechConfigs.Add(speechConfig);
            }
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

            var speechConfig = GetSpeechConfig();

            var ssmlMessage = ConvertToSsml(speechConfig, message);

            using (var result = await speechSynthesizer.StartSpeakingSsmlAsync(ssmlMessage))
            {
                logger.LogError($"Speech synthesized result: [{result.Reason}]");
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    logger.LogInformation($"Speech synthesized to speaker for text [{ssmlMessage}]");
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

        private string ConvertToSsml(SpeechConfig speechConfig, string message)
        {
            return @$"<speak version='1.0' xml:lang='en-US' xmlns='http://www.w3.org/2001/10/synthesis' xmlns:mstts='http://www.w3.org/2001/mstts'>
                        <voice name='{speechConfig.SpeechSynthesisVoiceName}'>
                            <mstts:viseme type='redlips_front'/>
                            {message}
                        </voice>
                    </speak>";
        }

        private SpeechConfig GetSpeechConfig()
        {
            var index = random.Next(speechConfigs.Count);

            logger.LogInformation("Language in use: {language}", speechConfigs[index].SpeechSynthesisLanguage);
            logger.LogInformation("Voice in use: {voice}", speechConfigs[index].SpeechSynthesisVoiceName);

            return speechConfigs[index];
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
