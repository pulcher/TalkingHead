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
        private readonly ILogger<Worker> logger;
        public readonly IConfiguration config;

        int buttonPin = 7;
        GpioController controller;
        List<string> sayings;
        Random random;
        ISayingResponse sayingResponse;

        public Worker(ISayingResponse sayingResponse, IConfiguration config, ILogger<Worker> logger)
        {
            this.sayingResponse = sayingResponse;
            this.logger = logger;
            this.config = config;

            var defaultLogLevel = config["Logging:LogLevel:Default"];
            logger.LogInformation($"defaultLogLevel = {defaultLogLevel}");

            var userName = config["TwitchBotConfiguration:UserName"];
            var accessToken = config["TwitchBotConfiguration:AccessToken"];

            var twitchBot = new TwitchBot(userName, accessToken, sayingResponse, logger);

            SetupGPIO();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var status = controller.Read(buttonPin);

                if (status == PinValue.Low)
                {
                    await sayingResponse.SaySomethingNice(sayingResponse.PickSaying());
                }

                await Task.Delay(100, stoppingToken);
            }
        }

        public void SetupGPIO()
        {
            logger.LogInformation("Setiing up GPIO...");
            controller = new GpioController(0, new RaspberryPi3Driver());

            controller.OpenPin (buttonPin, PinMode.InputPullUp);
        }
    }
}
