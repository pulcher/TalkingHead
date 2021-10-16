using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    internal class InspirationalCommand : IMbhCommand
    {
        private TwitchClient client;
        private ISayingResponse sayingResponse;
        private ILogger<Worker> logger;

        public InspirationalCommand(TwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
        {
            this.client = client;
            this.sayingResponse = sayingResponse;
            this.logger = logger;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            var message = GetRandomAnswer();

            client.SendMessage(cmd.Command.ChatMessage.Channel,
                $"Be inspired {cmd.Command.ChatMessage.Username}: {message}");
            sayingResponse.SaySomethingNice(message);
        }

        private string GetRandomAnswer()
        {
            return sayingResponse.PickSaying(Moods.Inspirational);
        }
    }
}