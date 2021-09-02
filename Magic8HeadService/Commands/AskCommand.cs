using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Magic8HeadService
{
    public class AskCommand : IMbhCommand
    {
        private ILogger<Worker> logger;
        private TwitchClient client;
        private SayingResponse sayingResponse;

        public AskCommand(TwitchClient client, SayingResponse sayingResponse, ILogger<Worker> logger)
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