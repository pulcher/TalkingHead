using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace MrBigHead.VoiceChecker
{
    public class Program
    {
        [AllowNull]
        public static IConfigurationRoot Configuration { get; set; }

        public static async Task Main(string[] args)
        {

            MySuperSecretStuff secretStuff = new ();
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();

            Configuration = builder.Build();
            IServiceCollection services = new ServiceCollection();

            var secretConfiguration = Configuration.GetSection(nameof(MySuperSecretStuff));

            var config = SpeechConfig.FromSubscription(secretConfiguration["SpeechSubscription"], 
                secretConfiguration["SpeechServiceRegion"]);
            // Note: the voice setting will not overwrite the voice element in input SSML.
            config.SpeechSynthesisVoiceName = "sw-TZ-DaudiNeural";

            //string text = "Can you programs understand the words that are coming out of my mouth?";

            var text = getText();

            // use the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                using (var result = await synthesizer.SpeakSsmlAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized for text [{text}]");
                        using var stream = AudioDataStream.FromResult(result);
                        await stream.SaveToWaveFileAsync(@"c:\temp\mbh\talk.wav");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }

        private static string getText()
        {
            using var sr = new StreamReader("speak-this.txt");

            var results = sr.ReadToEnd();

            return results;
            //return "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"en-US\">\r\n    <voice name=\"en-US-JennyNeural\">\r\n        This is the text that is spoken.\r\n    </voice>\r\n</speak>";
        }
    }
}
