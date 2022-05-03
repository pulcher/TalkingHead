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

        public SayCommand(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            if (cmd.Command.ChatMessage.IsSubscriber ||
                cmd.Command.ChatMessage.IsVip ||
                cmd.Command.ChatMessage.IsModerator)
            {
                var message = cmd.Command.ArgumentsAsString.Split(' ', 2);
                sayingResponse.SaySomethingNice(message[1]);
            }
            else
            {
                client.SendMessage(cmd.Command.ChatMessage.Channel,
                    $"Hey {cmd.Command.ChatMessage.Username}, the say command is for subscibers and vips only.");
            }
        }
    }
}