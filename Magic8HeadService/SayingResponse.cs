using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using MrBigHead.Services;
using MrBigHead.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public class SayingResponse : ISayingResponse
    {
        private Random random = new();
        private TwitchBotConfiguration twitchBotConfiguration;
        private readonly ISayingService sayingsService;
        private readonly IVoiceService voiceService;
        private readonly ILogger<Worker> logger;
        private IList<Saying> sayings;
        private IList<SpeechConfig> speechConfigs;
        private IList<SpeechConfigAssociated> speechConfigAssociated = new List<SpeechConfigAssociated>();
        private string defaultSpeechConfigVoiceName;
        private readonly SpeechSynthesizer speechSynthesizer;
        private readonly Dictionary<string, SpeechSynthesizer> speechSynthesizers = new Dictionary<string, SpeechSynthesizer>();

        public SayingResponse(TwitchBotConfiguration twitchBotConfiguration, ISayingService sayingsService, IVoiceService voicesService, ILogger<Worker> logger)
        {
            this.twitchBotConfiguration = twitchBotConfiguration;
            this.sayingsService = sayingsService;
            this.voiceService = voicesService;
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

            foreach (var voice in await GetVoices())
            {
                // create a new speech synth
                var speechConfig = SpeechConfig
                    .FromSubscription(twitchBotConfiguration.SpeechSubscription,
                    twitchBotConfiguration.SpeechServiceRegion);

                speechConfig.SpeechSynthesisLanguage = voice.Language;
                speechConfig.SpeechSynthesisVoiceName = voice.Name;

                if (voice.IsDefault)
                {
                    defaultSpeechConfigVoiceName = voice.Name;
                }

                //speechConfig.SetProperty(PropertyId.SpeechServiceResponse_RequestWordBoundary, "true");

                speechConfigs.Add(speechConfig);
            }
        }

        private async Task<IEnumerable<Voice>> GetVoices()
        {
            logger.LogInformation("Setiing up Voices...");

            var voices = await voiceService.GetAllVoicesAsync();

            logger.LogInformation($"voices count: {voices.Count}");

            return voices;

            //return new List<Voice>
            //{
            //    new Voice {Language = "en-GB", Name = "en-GB-RyanNeural", IsDefault = true},

            //    new Voice {Language = "en-US", Name = "cy-GB-AledNeural"},

            //    new Voice {Language = "en-PH", Name = "en-PH-RosaNeural"},
            //    new Voice {Language = "en-US", Name = "en-US-JennyNeural"},
            //    new Voice {Language = "en-PH", Name = "es-CU-BelkysNeural"},
            //    new Voice {Language = "en-US", Name = "es-CU-ManuelNeural"},

            //    new Voice {Language = "en-US", Name = "fil-PH-AngeloNeural"},
            //    new Voice {Language = "en-US", Name = "fr-CA-SylvieNeural"},
            //    new Voice {Language = "en-US", Name = "fr-CA-JeanNeural"},

            //    new Voice {Language = "en-US", Name = "it-IT-BenignoNeural"},
            //    new Voice {Language = "en-US", Name = "it-IT-FabiolaNeural"},
            //    new Voice {Language = "en-US", Name = "it-IT-IsabellaNeural"},

            //    new Voice {Language = "en-US", Name = "kk-KZ-DauletNeural"},
            //    new Voice {Language = "en-US", Name = "ru-RU-DmitryNeural"},
            //    new Voice {Language = "en-US", Name = "ru-RU-SvetlanaNeural"},
            //    new Voice {Language = "en-US", Name = "sl-SI-PetraNeural"},

            //    new Voice {Language = "en-US", Name = "sw-TZ-RehemaNeural"}

            //};
        }

        public async Task SetupSayingsAsync()
        {
            logger.LogInformation("Setiing up Sayings...");

            sayings = await sayingsService.GetAllSayingsAsync();

            logger.LogInformation($"saying count: {sayings.Count}");
        }

        public async Task SaySomethingNiceAsync(string message, ITwitchClient client, string channel, string username,
            CommandTrackerEntry commandTrackerEntity = null)
        {
            logger.LogInformation($"Saying: {message}");

            var speechConfig = GetSpeechConfig(commandTrackerEntity, username);

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

        private SpeechConfig GetSpeechConfig(CommandTrackerEntry commandTrackerEntity, string username)
        {
            SpeechConfig result = null;

            if (commandTrackerEntity == null)
            {
                result = speechConfigs.FirstOrDefault(s => s.SpeechSynthesisVoiceName == defaultSpeechConfigVoiceName);
            }
            else
            {
                var userSpeechConfigs = speechConfigAssociated.Where(u => u.Username == username).FirstOrDefault();

                if (userSpeechConfigs != null)
                {
                    result = userSpeechConfigs.SpeechConfig;
                }
                else
                {
                    var usedSpeechConfigs = speechConfigAssociated.Select(x => x.SpeechConfig.SpeechSynthesisVoiceName).ToList();

                    var avaialableConfigs = speechConfigs.Where(s =>
                        s.SpeechSynthesisVoiceName != defaultSpeechConfigVoiceName
                        && !usedSpeechConfigs.Contains(s.SpeechSynthesisVoiceName));

                    result = avaialableConfigs.Skip(random.Next(avaialableConfigs.Count())).First();

                    speechConfigAssociated.Add(new SpeechConfigAssociated { Username = username, SpeechConfig = result });
                }
            }

            logger.LogInformation("Language in use: {language}", result.SpeechSynthesisLanguage);
            logger.LogInformation("Voice in use: {voice}", result.SpeechSynthesisVoiceName);

            return result;
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

        public bool ResetVoiceForUser(string username)
        {
            logger.LogInformation("{username}", username);

            // see if the user exists
            var userSpeechConfigs = speechConfigAssociated.Where(u => u.Username == username).FirstOrDefault();

            if (userSpeechConfigs != null)
            {
                speechConfigAssociated.Remove(userSpeechConfigs);
                return true;
            }

            return false;
        }
    }
}
