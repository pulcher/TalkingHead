using Magic8HeadService.Services;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public class SayCommand : IMbhCommand
    {
        private readonly ILogger<Worker> logger;
        private readonly ITwitchClient client;
        private ISayingResponse sayingResponse;
        private IMessageChecker messageChecker;
        private readonly ICommandTracker commandTracker;

        public SayCommand(ITwitchClient client, ISayingResponse sayingResponse,
                          IMessageChecker messageChecker, ICommandTracker commandTracker,
                          ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.messageChecker = messageChecker;
            this.commandTracker = commandTracker;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            if (cmd.Command.ChatMessage.IsSubscriber ||
                cmd.Command.ChatMessage.IsVip ||
                cmd.Command.ChatMessage.IsModerator)
            {
                var message = cmd.Command.ArgumentsAsString.Split(' ', 2);
                var username = cmd.Command.ChatMessage.Username;
                var channel = cmd.Command.ChatMessage.Channel;

                var commandTrackerEntity = commandTracker.Add(username, "say");

                sayingResponse.SaySomethingNiceAsync(messageChecker.CheckMessage(message[1]), client, channel, username, commandTrackerEntity)
                    .Wait();
            }
            else
            {
                client.SendMessage(cmd.Command.ChatMessage.Channel,
                    $"Hey {cmd.Command.ChatMessage.Username}, the say command is for subscribers and vips only.");
            }

            var currentSessionCommands = commandTracker.GetSessionCommands("say");
            
            var check = string.Empty;
        }
    }
}