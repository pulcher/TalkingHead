using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    internal class DadCommand : IMbhCommand
    {
        private TwitchClient client;
        private ISayingResponse sayingResponse;
        private readonly IDadJokeService dadJokeService;
        private ILogger<Worker> logger;

        public DadCommand(TwitchClient client, ISayingResponse sayingResponse, IDadJokeService dadJokeService, ILogger<Worker> logger)
        {
            this.client = client;
            this.sayingResponse = sayingResponse;
            this.dadJokeService = dadJokeService;
            this.logger = logger;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            try
            {
                var dadJoke = dadJokeService.GetDadJoke().Result;

                sayingResponse.SaySomethingNice(dadJoke.Setup);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"Q: {dadJoke.Setup}");

                Task.Delay(5000).Wait();

                sayingResponse.SaySomethingNice(dadJoke.Punchline);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"A: {dadJoke.Punchline}");
            }
            catch (Exception ex)
            {
                sayingResponse.SaySomethingNice(ex.Message);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"We've got problem {ex.Message}");
            }
        }

    }
}
