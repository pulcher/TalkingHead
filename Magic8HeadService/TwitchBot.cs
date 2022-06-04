using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System;
using System.Linq;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public class TwitchBot
    {
        private readonly ITwitchClient client;
        private readonly ICommandMbhToTwitch helpCommandReal;
        private readonly Dictionary<string, ICommandMbhToTwitch> dictOfCommands;

        private readonly ILogger<Worker> logger;
        private readonly ISayingResponse sayingResponse;
        private readonly IDadJokeService dadJokeService;

        public TwitchBot(ITwitchClient client, ConnectionCredentials clientCredentials, TwitchBotConfiguration twitchBotConfiguration,
            ISayingResponse sayingResponse, IDadJokeService dadJokeService, IEnumerable<ICommandMbhToTwitch> listOfCommands, 
            ICommandMbhTwitchHelp helpCommand, ILogger<Worker> logger)
        {
            this.client = client;

            this.helpCommandReal = (ICommandMbhToTwitch)helpCommand;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.dadJokeService = dadJokeService;

            var listOfNames = listOfCommands.Select(x => x.Name);
            this.logger.LogInformation($"-------------- List of Names :  {string.Join(',', listOfNames)}");
            dictOfCommands = listOfCommands
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

	    this.logger.LogInformation($"================== dictOfCommand Count: {dictOfCommands.Count}");

            this.client.Initialize(clientCredentials, twitchBotConfiguration.ChannelName);

            this.client.OnLog += Client_OnLog;
            this.client.OnJoinedChannel += Client_OnJoinedChannel;
            this.client.OnMessageReceived += Client_OnMessageReceived;
            this.client.OnWhisperReceived += Client_OnWhisperReceived;
            this.client.OnNewSubscriber += Client_OnNewSubscriber;
            this.client.OnReSubscriber += Client_OnReSubscriber;
            this.client.OnConnected += Client_OnConnected;
            this.client.OnChatCommandReceived += Client_OnChatCommandReceived;
            this.client.OnRaidNotification += Client_OnRaidNotification;

            var clientResult = this.client.Connect();
	    this.logger.LogInformation($"============ client connected");
        }

        public void Client_OnLog(object sender, OnLogArgs e)
        {
            logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        public void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        public void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey programs! Let's talk!");
            this.client.SendMessage(e.Channel, "Hey programs! Let's talk!");
        }

        public void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
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

            sayingResponse.SaySomethingNice(message).Wait();

            this.client.SendMessage(e.Channel, message);

        }

        public void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            var message = $"Thanks {e.ReSubscriber.DisplayName} for the re-sub!";

            if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                message += " So kind of you to use your Twitch Prime on my channel!";

            sayingResponse.SaySomethingNice(message).Wait();

            this.client.SendMessage(e.Channel, message);
        }

        public void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            var message = $"Thanks for the RAID {e.RaidNotification.DisplayName}!  How was your stream?";

            sayingResponse.SaySomethingNice(message).Wait();

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
