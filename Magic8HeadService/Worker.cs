using Magic8HeadService.MqttHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace Magic8HeadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IServiceProvider service;
        public readonly IConfiguration config;

        readonly int buttonPin = 7;
        GpioController controller;
        readonly ITwitchClient twitchClient;
        readonly ConnectionCredentials connectionCredentials;
        readonly ISayingResponse scopedSayingResponse;
        readonly IDadJokeService scopedDadJokeService;
        readonly MqttFactory scopedMqttFactory;
        readonly IMessageStackService scopedMessageStackService;

        public Worker(IServiceProvider service, IConfiguration config, TwitchBotConfiguration twitchBotConfiguration,
             ILogger<Worker> logger)
        {
            this.logger = logger;
            this.service = service;
            this.config = config;

            var defaultLogLevel = config["Logging:LogLevel:Default"];
            logger.LogInformation($"defaultLogLevel = {defaultLogLevel}");

            using var scope = service.CreateScope();

            twitchClient =
                service.GetRequiredService<ITwitchClient>();

            connectionCredentials =
                service.GetRequiredService<ConnectionCredentials>();

            scopedSayingResponse =
                scope.ServiceProvider
                    .GetRequiredService<ISayingResponse>();
            scopedDadJokeService =
                scope.ServiceProvider
                    .GetRequiredService<IDadJokeService>();

            scopedMqttFactory =
                scope.ServiceProvider
                    .GetRequiredService<MqttFactory>();

            scopedMessageStackService =
                scope.ServiceProvider
                .GetRequiredService<IMessageStackService>();

            var listOfCommands = scope.ServiceProvider.GetServices<ICommandMbhToTwitch>();
            var helpCommand = scope.ServiceProvider.GetService<ICommandMbhTwitchHelp>();
            var mqttHandlers = scope.ServiceProvider.GetServices<IMqttHandler>();

            var twitchBot = new TwitchBot(twitchClient, connectionCredentials, twitchBotConfiguration,
                scopedSayingResponse, scopedDadJokeService, listOfCommands, 
                helpCommand, scopedMqttFactory, mqttHandlers, scopedMessageStackService,
                logger);

            SetupGPIO();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ExecuteAsync fired...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var status = controller?.Read(buttonPin);

                if (status == PinValue.Low)
                {
                    logger.LogInformation("saying words...");
                    var message = scopedSayingResponse.PickSaying();
                    logger.LogInformation($"ExecuteAsync: picked saying {message}");
                    await scopedSayingResponse.SaySomethingNiceAsync(message, null, string.Empty, string.Empty);
                }

                await Task.Delay(100, stoppingToken);
            }
        }

        public void SetupGPIO()
        {
            logger.LogInformation("Setiing up GPIO...");

            try
            {
                controller = new GpioController(0, new RaspberryPi3Driver());
                controller.OpenPin(buttonPin, PinMode.InputPullUp);
            }
            catch (Exception e)
            {
                logger.LogInformation(e, "GPIO not available");
            }
        }
    }
}
