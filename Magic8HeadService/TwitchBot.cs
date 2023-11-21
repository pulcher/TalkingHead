using Magic8HeadService.MqttHandlers;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace Magic8HeadService
{
    public class TwitchBot
    {
        private readonly ITwitchClient client;
        private readonly IMqttClient mqttClient;
        private readonly ICommandMbhToTwitch helpCommandReal;
        private readonly Dictionary<string, ICommandMbhToTwitch> dictOfCommands;
        private readonly List<IMqttHandler> mqttHandlers1= new List<IMqttHandler>();

        private readonly ILogger<Worker> logger;
        private readonly ISayingResponse sayingResponse;
        private readonly IDadJokeService dadJokeService;
        private readonly IMessageStackService messageStackService;

        public TwitchBot(ITwitchClient client, ConnectionCredentials clientCredentials, TwitchBotConfiguration twitchBotConfiguration,
            ISayingResponse sayingResponse, IDadJokeService dadJokeService, IEnumerable<ICommandMbhToTwitch> listOfCommands, 
            ICommandMbhTwitchHelp helpCommand, MqttFactory mqttFactory, IEnumerable<IMqttHandler> mqttHandlers, 
            IMessageStackService messageStackService, ILogger<Worker> logger)
        {
            this.client = client;

            this.helpCommandReal = (ICommandMbhToTwitch)helpCommand;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.dadJokeService = dadJokeService;
            this.messageStackService = messageStackService;
            var listOfNames = listOfCommands.Select(x => x.Name);
            this.logger.LogInformation($"-------------- List of Names :  {string.Join(',', listOfNames)}");
            dictOfCommands = listOfCommands
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            this.logger.LogInformation($"================== dictOfCommand Count: {dictOfCommands.Count}");

            SetupTwitchClient(clientCredentials, twitchBotConfiguration);

            mqttClient = mqttFactory.CreateMqttClient();

            var mqttCreds = new MqttClientCredentials("mbh", Encoding.ASCII.GetBytes("mbh"));

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("broker1.killercomputing.com")
                .WithCredentials(mqttCreds)
                .Build();

            // Setup mqttMessageWrapper handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine("Received application mqttMessageWrapper.");
                //e.DumpToConsole();

                var mqttMessageWrapper = new MqttHandlerMessage(e.ApplicationMessage.Topic.ToString(), 
                    e.ApplicationMessage.PayloadSegment);

                var handlers = mqttHandlers.Where(h => h.CanHandle(mqttMessageWrapper));
                foreach (var handler in handlers)
                {
                    handler.Handle(mqttMessageWrapper);
                }
                
                if (mqttMessageWrapper.Errors.Any())
                {
                    Console.WriteLine("something blew up.");
                }

                e.IsHandled = true;

                return Task.CompletedTask;
            };

            mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).Wait();

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mbh/#");
                    })
                .Build();

            mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).Wait();

            Console.WriteLine("MQTT client subscribed to topic.");
        }

        private void SetupTwitchClient(ConnectionCredentials clientCredentials, TwitchBotConfiguration twitchBotConfiguration)
        {
            this.client.Initialize(clientCredentials, twitchBotConfiguration.ChannelName);

            this.client.OnChatCommandReceived += Client_OnChatCommandReceived;
            this.client.OnConnected += Client_OnConnected;
            this.client.OnDisconnected += Client_OnDisconnected;
            this.client.OnJoinedChannel += Client_OnJoinedChannel;
            this.client.OnLog += Client_OnLog;
            this.client.OnMessageReceived += Client_OnMessageReceived;
            this.client.OnModeratorJoined += Client_OnModeratorJoined;
            this.client.OnModeratorLeft += Client_OnModeratorLeft;
            this.client.OnNewSubscriber += Client_OnNewSubscriber;
            this.client.OnRaidNotification += Client_OnRaidNotification;
            this.client.OnReSubscriber += Client_OnReSubscriber;
            this.client.OnWhisperReceived += Client_OnWhisperReceived;

            var clientResult = this.client.Connect();
            this.logger.LogInformation($"============ client connected");
        }

        private void Client_OnModeratorLeft(object sender, OnModeratorLeftArgs e)
        {
            logger.LogInformation("Moderator {moderator} left the channel. ", e.Username);
        }

        public void Client_OnModeratorJoined(object sender, OnModeratorJoinedArgs e)
        {
            var message = $"Hey programs, sword-bearing moderator {e.Username} has joined! Hide yo' bugs, hide yo' bits!";
            logger.LogInformation(message);

            sayingResponse.SaySomethingNiceAsync(message, client, e.Channel, e.Username).Wait();

            this.client.SendMessage(e.Channel, message);
        }

        public void Client_OnLog(object sender, OnLogArgs e)
        {
            logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        public void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInformation($"Connected to {e.AutoJoinChannel}");
            logger.LogInformation($"Connected as {e.BotUsername}");
        }

        public void Client_OnDisconnected(object sender, OnDisconnectedEventArgs e)
        {
            logger.LogInformation($"======== Disconnected ===================");
        }

        public void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogInformation("Hey programs! Let's talk and make stuff, break stuff, and learn stuff!");
            this.client.SendMessage(e.Channel, "Hey programs! Let's talk and make stuff, break stuff, and learn stuff!");
        }

        public void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (!e.ChatMessage.IsVip
                && !e.ChatMessage.IsModerator
                && !e.ChatMessage.IsSubscriber
                && !e.ChatMessage.IsMe
                && !e.ChatMessage.IsStaff
                && !e.ChatMessage.Message.StartsWith('!'))
            {
                messageStackService.PutMessage(e.ChatMessage);
            }

            if (e.ChatMessage.Message.Contains("badword"))
                this.client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        public void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
            this.client.SendWhisper(e.WhisperMessage.Username, "Hey! Say sweet nothings to me!");
        }

        public void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            var message = $"Welcome {e.Subscriber.DisplayName} to the my lab!";

            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                message += " So kind of you to use your Twitch Prime on my channel!";

            sayingResponse.SaySomethingNiceAsync(message, client, e.Channel, string.Empty).Wait();

            this.client.SendMessage(e.Channel, message);

        }

        public void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            var message = $"Thanks {e.ReSubscriber.DisplayName} for the re-sub!";

            if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                message += " So kind of you to use your Twitch Prime on my channel!";

            sayingResponse.SaySomethingNiceAsync(message, client, e.Channel, string.Empty).Wait();

            this.client.SendMessage(e.Channel, message);
        }

        public void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            var message = $"Thanks for the RAID {e.RaidNotification.DisplayName}!  How was your stream?";

            sayingResponse.SaySomethingNiceAsync(message, client, e.Channel, string.Empty).Wait();

            this.client.SendMessage(e.Channel, message);
        }

        public void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
            var chatSubscription = e.Command.ChatMessage.IsSubscriber;

            logger.LogInformation($"command is: {e.Command.CommandText.ToLower()}");
            logger.LogInformation($"args is null? : '{e.Command.ArgumentsAsList == null}'");

            var dictCommand = dictOfCommands.GetValueOrDefault(e.Command.CommandText, helpCommandReal);
            dictCommand.Handle(e);
		}
    }
}
