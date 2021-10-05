using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Internal;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Logging;

namespace Magic8HeadService
{
    public class TwitchBot
    {
        TwitchClient client;
        ILogger<Worker> logger;
        ISayingResponse sayingResponse;

        public TwitchBot(string userName, string accessToken, ISayingResponse sayingResponse,
            ILogger<Worker> logger)
        {
            this.logger = logger;
            this.sayingResponse = sayingResponse;

            var credentials = new ConnectionCredentials(userName, accessToken);
	        var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30),
                    UseSsl = true
                };
            var customClient = new WebSocketClient(clientOptions);

            client = new TwitchClient(customClient);
            client.Initialize(credentials, "haroldpulcher");

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnReSubscriber += Client_OnReSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
            client.OnRaidNotification += Client_OnRaidNotification;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey programs! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey programs! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("badword"))
                client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
            client.SendWhisper(e.WhisperMessage.Username, "Hey! Say sweet nothings to me!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            var message = $"Welcome {e.Subscriber.DisplayName} to the my lab!";

            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                message += " So kind of you to use your Twitch Prime on my channel!";

            sayingResponse.SaySomethingNice(message).Wait();

            client.SendMessage(e.Channel, message);

        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            var message = $"Thanks {e.ReSubscriber.DisplayName} for the re-sub!";

            if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                message += " So kind of you to use your Twitch Prime on my channel!";

            sayingResponse.SaySomethingNice(message).Wait();

            client.SendMessage(e.Channel, message);
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            var message = $"Thanks for the RAID {e.RaidNotification.DisplayName}!  How was your stream?";

            sayingResponse.SaySomethingNice(message).Wait();

            client.SendMessage(e.Channel, message);
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{
            // client.SendMessage(e.Command.ChatMessage.Channel, $"I have received {e.Command.ChatMessage.Username}'s command.  Are you postive you want me to: {e.Command.ArgumentsAsString}");

            IMbhCommand action = new NullCommand(logger);

            var chatSubscription = e.Command.ChatMessage.IsSubscriber;

            logger.LogInformation($"command is: {e.Command.CommandText.ToLower()}");
            logger.LogInformation($"args is null? : '{e.Command.ArgumentsAsList == null}'");

            switch (e.Command.ArgumentsAsList.FirstOrDefault()?.ToLower())
            {
                case AvailableCommands.Help:
                case "":
                case null:
                    action = new HelpCommand(client, logger);
                    break;
                case AvailableCommands.Ask:
                	action = new AskCommand(client, sayingResponse, logger);
                	break;
                case AvailableCommands.Say:
                    action = new SayCommand(client, sayingResponse, logger);
                    break;
            }

            action.Handle(e);
		}
    }
}
