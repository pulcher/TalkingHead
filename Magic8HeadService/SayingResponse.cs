using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrBigHead.Services;
using MrBigHead.Shared;

namespace Magic8HeadService
{
    public class SayingResponse : ISayingResponse
    {
        private Random random;
        private IConfiguration config;
        private readonly ISayingService sayingsService;
        private ILogger<Worker> logger;
        private List<Saying> sayings;
        private SpeechSynthesizer speechSynthesizer;

        public string Attitude { get; set; }

        public SayingResponse(IConfiguration config, ISayingService sayingsService, ILogger<Worker> logger)
        {
            this.config = config;
            this.sayingsService = sayingsService;
            this.logger = logger;

            // create a new speech synth
            var speechConfig = SpeechConfig
                .FromSubscription(config["TwitchBotConfiguration:SpeechSubscription"], config["TwitchBotConfiguration:SpeechServiceRegion"]);
            speechConfig.SpeechSynthesisLanguage = "en-GB";
            speechConfig.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
            logger.LogInformation($"Here is the SpeechSynthesisLanguage: {speechConfig.SpeechSynthesisLanguage}");
            logger.LogInformation($"Here is the SpeechSynthesisVoiceName: {speechConfig.SpeechSynthesisVoiceName}");

            speechSynthesizer = new SpeechSynthesizer(speechConfig);

            SetupSayings();
        }

        public void SetupSayings()
        {
            logger.LogInformation("Setiing up Sayings...");

            random = new Random();
            sayings = sayingsService.GetAllSayings();

            logger.LogInformation($"saying count: {sayings.Count}");
            //sayings = new List<string>
            //{
            //    "Greetings Programs!",
            //    "Have a nice Day",
            //    "Ralph helps sooooooo much!",
            //    "I think you shouldn't have gotten out of bed today!",
            //    "Yeah, Baby!",
            //    "I am positive it is gonna suck!",
            //    "Yes",
            //    "No",
            //    "It depends",
            //    "You want frys with that?",
            //    "You need to to rephrase your question.",
            //    "If had a dollar for every smart thing you say. I'd be poor.",
            //    "Silence is golden. Duct tape is silver.",
            //    "Id agree with you but then wed both be wrong.",
            //    "Without a doubt!",
            //    "Make it so!",
            //    "That is the best idea I have ever heard.",
            //    "Your trending upward!",
            //    "I would find a scapegoat!",
            //    "I would wait till tomorrow on that.",
            //    "You need to turn that up to 11!",
            //    "How would your mother answer that?",
            //    "You can never touch the piece of water twice.",
            //    "Up is down, down is up, and sideways is straight ahead!",
            //    "I will answer you tomorrow.",
            //    "Catastrophies are always emminent!",
            //    "Dogsasters happen at the worst of times!",
            //    "Everything happens for a reason, sometimes the reason is you're stupid and make bad decisions."
            //};
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

            // using (Process fliteProcess = new Process())
            // {
            //     var voiceDir = "/home/pi/work/speechTest/voices/";
            //     var voice = "cmu_us_aew.flitevox";
            //     var exec = "/usr/local/bin/flite";
            //     var args = $"-voice awb";

            //     fliteProcess.StartInfo.FileName = exec;
            //     fliteProcess.StartInfo.Arguments = args;
            //     fliteProcess.StartInfo.UseShellExecute = false;
            //     fliteProcess.StartInfo.RedirectStandardInput = true;

            //     fliteProcess.Start();

            //     var streamWriter = fliteProcess.StandardInput;

            //     var inputText = message; // PickSaying();
            //     logger.LogInformation($"Saying: {inputText}");

            //     streamWriter.WriteLine(inputText);
            //     streamWriter.Close();

            //     fliteProcess.WaitForExit();
            // }

            return;
        }

        public string PickSaying()
        {
            logger.LogInformation("PickSaying: ");
            var index = random.Next(sayings.Count);
            logger.LogInformation($"PickSaying: count: {sayings.Count}, random: {index}");
            var picked = sayings[index];

            return picked.Quip;
        }
    }
}