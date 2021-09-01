using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Magic8HeadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public readonly IConfiguration config;
        
        int buttonPin = 7;
        GpioController controller;
        List<string> sayings;
        Random random;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            this.config = config;

            var defaultLogLevel = config["Logging:LogLevel:Default"];
            _logger.LogInformation($"defaultLogLevel = {defaultLogLevel}");

            var userName = config["TwitchBotConfiguration:UserName"];
            var accessToken = config["TwitchBotConfiguration:AccessToken"];

            var twitchBot = new TwitchBot(userName, accessToken, _logger);

            SetupGPIO();

            SetupSayings();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var status = controller.Read(buttonPin);
                
                if (status == PinValue.Low)
                    SaySomethingNice();

                await Task.Delay(100, stoppingToken);
            }
        }

        public void SetupGPIO()
        {
            _logger.LogInformation("Setiing up GPIO...");
            controller = new GpioController(0, new RaspberryPi3Driver());

            controller.OpenPin (buttonPin, PinMode.InputPullUp);
        }

        public void SetupSayings()
        {
            _logger.LogInformation("Setiing up Sayings...");

            random = new Random();
            sayings = new List<string>
            {
                "Greetings Programs!",
                "Have a nice Day",
                "Ralph helps sooooooo much!",
                "I think you shouldn't have gotten out of bed today!",
                "Yeah, Baby!",
                "I am positive it is gonna suck!",
                "Yes",
                "No",
                "It depends",
                "You want frys with that?",
                "You need to to rephrase your question.",
                "If had a dollar for every smart thing you say. I'd be poor.",
                "Silence is golden. Duct tape is silver.",
                "Id agree with you but then wed both be wrong.",
                "Without a doubt!",
                "Make it so!",
                "That is the best idea I have ever heard.",
                "Your trending upward!",
                "I would find a scapegoat!",
                "I would wait till tomorrow on that.",
                "You need to turn that up to 11!",
                "How would your mother answer that?",
                "You can never touch the piece of water twice.",
                "Up is down, down is up, and sideways is straight ahead!",
                "I will answer you tomorrow.",
                "Catastrophies are always emminent!",
                "Dogsasters happen at the worst of times!",
                "Everything happens for a reason, sometimes the reason is you're stupid and make bad decisions."
            };
        }

        public void SaySomethingNice()
        {
            using (Process fliteProcess = new Process())
            {
                var voiceDir = "/home/pi/work/speechTest/voices/";
                var voice = "cmu_us_aew.flitevox";
                var exec = "/usr/local/bin/flite";
                var args = $"-voice awb";

                fliteProcess.StartInfo.FileName = exec;
                fliteProcess.StartInfo.Arguments = args;
                fliteProcess.StartInfo.UseShellExecute = false;
                fliteProcess.StartInfo.RedirectStandardInput = true;

                fliteProcess.Start();

                var streamWriter = fliteProcess.StandardInput;
                
                var inputText = PickSaying();
                _logger.LogInformation($"Saying: {inputText}");

                streamWriter.WriteLine(inputText);
                streamWriter.Close();

                fliteProcess.WaitForExit();
            }

            return;
        }

        public string PickSaying()
        {
            var index = random.Next(sayings.Count);
            _logger.LogInformation($"count: {sayings.Count}, random: {index}");
            var picked = sayings[index];

            return picked;
        }        
    }
}
