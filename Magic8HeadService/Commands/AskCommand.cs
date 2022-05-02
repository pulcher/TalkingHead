using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public class AskCommand : IMbhCommand
    {
        private readonly ILogger logger;
        private readonly ITwitchClient client;
        private readonly ISayingResponse sayingResponse;
        private readonly string mood;

        public AskCommand(ITwitchClient client, ISayingResponse sayingResponse, string mood, ILogger logger)
        {
            if (string.IsNullOrEmpty(mood))
            {
                mood = Moods.Snarky;
            }

            this.client = client;
            this.logger = logger;
            this.sayingResponse = sayingResponse;
            this.mood = mood;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            var message = GetRandomAnswer().ToLower();

            client.SendMessage(cmd.Command.ChatMessage.Channel,
                $"Hey {cmd.Command.ChatMessage.Username}, {message}");
            sayingResponse.SaySomethingNice(message);
        }

        private string GetRandomAnswer()
        {
            return sayingResponse.PickSaying(mood);
        }
    }
}
