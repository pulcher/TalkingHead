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

        public SayCommand(ITwitchClient client, ISayingResponse sayingResponse, IMessageChecker messageChecker, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.messageChecker = messageChecker;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            if (cmd.Command.ChatMessage.IsSubscriber ||
                cmd.Command.ChatMessage.IsVip ||
                cmd.Command.ChatMessage.IsModerator)
            {
                var message = cmd.Command.ArgumentsAsString.Split(' ', 2);
                sayingResponse.SaySomethingNice(messageChecker.CheckMessage(message[1]));
            }
            else
            {
                client.SendMessage(cmd.Command.ChatMessage.Channel,
                    $"Hey {cmd.Command.ChatMessage.Username}, the say command is for subscribers and vips only.");
            }
        }
    }
}