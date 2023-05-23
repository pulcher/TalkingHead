using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    internal class DadCommand : IMbhCommand
    {
        private ITwitchClient client;
        private ISayingResponse sayingResponse;
        private readonly IDadJokeService dadJokeService;
        private ILogger<Worker> logger;
        private string alternateSite = "https://karljoke.herokuapp.com/jokes/random"; //= string.Empty;

        public DadCommand(ITwitchClient client, ISayingResponse sayingResponse, IDadJokeService dadJokeService, ILogger<Worker> logger)
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
                var dadJoke = dadJokeService.GetDadJoke(alternateSite).Result;

                sayingResponse.SaySomethingNiceAsync(dadJoke.Setup, client, cmd.Command.ChatMessage.Channel, cmd.Command.ChatMessage.Username);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"Q: {dadJoke.Setup}");

                Task.Delay(5000).Wait();

                sayingResponse.SaySomethingNiceAsync(dadJoke.Punchline, client, cmd.Command.ChatMessage.Channel, cmd.Command.ChatMessage.Username);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"A: {dadJoke.Punchline}");
            }
            catch (Exception ex)
            {
                alternateSite = "https://karljoke.herokuapp.com/jokes/random";

                sayingResponse.SaySomethingNiceAsync(ex.Message, client, cmd.Command.ChatMessage.Channel, cmd.Command.ChatMessage.Username);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"We've got problem {ex.Message}");
            }
        }

    }
}
