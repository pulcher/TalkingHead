using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public class AskCommand : IMbhCommand
    {
        private readonly ILogger logger;
        private readonly TwitchClient client;
        private readonly ISayingResponse sayingResponse;

        public AskCommand(TwitchClient client, ISayingResponse sayingResponse, ILogger logger)
        {
            this.client = client;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            var message = GetRandomAnswer();

            client.SendMessage(cmd.Command.ChatMessage.Channel,
                $"Hey {cmd.Command.ChatMessage.Username}, here is your answer: {message}");
            sayingResponse.SaySomethingNice(message);
        }

        private string GetRandomAnswer()
        {
            return sayingResponse.PickSaying();
        }
    }
}