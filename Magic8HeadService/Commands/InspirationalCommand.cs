using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    internal class InspirationalCommand : IMbhCommand
    {
        private ITwitchClient client;
        private ISayingResponse sayingResponse;
        private ILogger<Worker> logger;

        public InspirationalCommand(ITwitchClient client, ISayingResponse sayingResponse, ILogger<Worker> logger)
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
            sayingResponse.SaySomethingNiceAsync(message, client, cmd.Command.ChatMessage.Channel, cmd.Command.ChatMessage.Username);
        }

        private string GetRandomAnswer()
        {
            return sayingResponse.PickSaying(Moods.Inspirational);
        }
    }
}