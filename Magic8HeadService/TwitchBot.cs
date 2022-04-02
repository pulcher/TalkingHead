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
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Microsoft.Extensions.Configuration;

namespace Magic8HeadService
{
    public class TwitchBot
    {
        private readonly TwitchClient client;
        private readonly Dictionary<string, Action<OnChatCommandReceivedArgs>> commands;

        private readonly ICommandMbhToTwitch helpCommandReal;
        private readonly Dictionary<string, ICommandMbhToTwitch> dictOfCommands;
        // private readonly ICommandMbhToTwitch CommandMbhHelp = 
        //                                         (e) => Console.WriteLine("--------- Command Mbh Help -----");

        private readonly Action<OnChatCommandReceivedArgs> HelpCommand = 
                                                (e) => Console.WriteLine("****** help ******");
        private readonly ILogger<Worker> logger;
        private readonly ISayingResponse sayingResponse;
        private readonly IDadJokeService dadJokeService;
        private string mood = Moods.Snarky;

        public TwitchBot(TwitchClient client, ConnectionCredentials clientCredentials, string channelName,
            IConfiguration config,
            ISayingResponse sayingResponse, IDadJokeService dadJokeService, IEnumerable<ICommandMbhToTwitch> listOfCommands,
            ICommandMbhTwitchHelp helpCommand,
            ILogger<Worker> logger)
        {

            this.client = client;
            //logger.LogInformation($"helpCommand: {helpCommand?.Name ?? "uggggggggggggggghhhhhhhh!!!!!!1"}");
            this.helpCommandReal = (ICommandMbhToTwitch)helpCommand;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.dadJokeService = dadJokeService;
            // var credentials = new ConnectionCredentials(userName, accessToken);
	        // var clientOptions = new ClientOptions
            //     {
            //         MessagesAllowedInPeriod = 750,
            //         ThrottlingPeriod = TimeSpan.FromSeconds(30),
            //         UseSsl = true
            //     };
            // var customClient = new WebSocketClient(clientOptions);

            var listOfNames = listOfCommands.Select(x => x.Name);
            Console.WriteLine($"-------------- List of Names :  {string.Join(',', listOfNames)}");
            dictOfCommands = listOfCommands
                .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

            commands = new Dictionary<string, Action<OnChatCommandReceivedArgs>>();
            CommandSetup();

            // client = new TwitchClient(customClient);
            this.client.Initialize(clientCredentials, channelName);

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

            logger.LogInformation($"Yes{clientResult}, Hugo I am getting past all of these.........!!!!!!!!!!!   Really!!!!");
        }

        private void CommandSetup() 
        {
            var mbhAction = new Action<OnChatCommandReceivedArgs>(x => Console.WriteLine("****** mbh"));
            var uptimeAction = new Action<OnChatCommandReceivedArgs>(x => Console.WriteLine("****** uptime"));
            commands.Add(ActionCommands.Mbh, mbhAction);
            commands.Add(ActionCommands.Uptime, mbhAction);
            //commands.Add(ActionCommands.HelpCommand, HelpCommand);
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
            Console.WriteLine("Hey programs! I am a test bot connected via TwitchLib!");
            this.client.SendMessage(e.Channel, "Hey programs! I am a test bot connected via TwitchLib!");
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
            // client.SendMessage(e.Command.ChatMessage.Channel, $"I have received {e.Command.ChatMessage.Username}'s command.  Are you postive you want me to: {e.Command.ArgumentsAsString}");

            IMbhCommand action = new NullCommand(logger);

            var chatSubscription = e.Command.ChatMessage.IsSubscriber;

            logger.LogInformation($"command is: {e.Command.CommandText.ToLower()}");
            logger.LogInformation($"args is null? : '{e.Command.ArgumentsAsList == null}'");

            var dictCommand = dictOfCommands.GetValueOrDefault(e.Command.CommandText, helpCommandReal);

            // logger.LogInformation($"resolved command: {dictCommand?.Name ?? "BOOOOOOOMMMMM!!!!!"}");
            // logger.LogInformation($"helpcommand name: {helpCommandReal?.Name ?? "Booooommmmmm Part Duex!"}");
            dictCommand.Handle(e);

            // var actionCommand = commands.GetValueOrDefault(e.Command.CommandText.ToLower(), HelpCommand);
            // actionCommand(e);

            // switch (e.Command.CommandText.ToLower())
            // {
            //     case ActionCommands.Uptime:
            //         logger.LogInformation($"Gotta report some uptime...");
            //         return;
            //     case ActionCommands.Mbh:
            //         switch (e.Command.ArgumentsAsList.FirstOrDefault()?.ToLower())
            //             {
            //                 case AvailableCommands.Help:
            //                 // case null:
            //                     action = new HelpCommand(client, logger);
            //                     break;
            //                 case AvailableCommands.Ask:
            //                     action = new AskCommand(client, sayingResponse, mood, logger);
            //                     break;
            //                 case AvailableCommands.Dad:
            //                     action = new DadCommand(client, sayingResponse, dadJokeService, logger);
            //                     break;
            //                 case AvailableCommands.Inspire:
            //                     action = new InspirationalCommand(client, sayingResponse, logger);
            //                     break;
            //                 case AvailableCommands.Say:
            //                     action = new SayCommand(client, sayingResponse, logger);
            //                     break;
            //                 default:
            //                     break;
            //             }
                        
            //             action.Handle(e);
            //         return;
            //     default:
            //         return;
            // }
		}
    }
}
