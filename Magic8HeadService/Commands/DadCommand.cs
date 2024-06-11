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
        private TimeSpan coolDownTimeSpan = TimeSpan.FromSeconds(15);
        private DateTime coolDown = DateTime.MinValue;
        private ILogger<Worker> logger;
        private string alternateSite = "https://karljoke.herokuapp.com/jokes/random"; //= string.Empty;

        public DadCommand(ITwitchClient client, ISayingResponse sayingResponse, IDadJokeService dadJokeService, DateTime coolDown, ILogger<Worker> logger)
        {
            this.client = client;
            this.sayingResponse = sayingResponse;
            this.dadJokeService = dadJokeService;
            this.logger = logger;
            this.coolDown = coolDown;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            if (CanExecute())
            {
                //coolDown = DateTime.UtcNow.AddMinutes(1);
                //logger.Log(LogLevel.Debug, "Setting the next available DadJoke to {coolDown}", coolDown);

                var dadJoke = dadJokeService.GetDadJoke();

                sayingResponse.SaySomethingNiceAsync(dadJoke, client, cmd.Command.ChatMessage.Channel, cmd.Command.ChatMessage.Username);
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"Q: {dadJoke}");
            }
            else
            {
                client.SendMessage(cmd.Command.ChatMessage.Channel, $"Pull my finger is brewin'");
            }
        }

        public bool CanExecute()
        {
            var currentTime = DateTime.UtcNow;
            if ( currentTime < coolDown )
            {
                return false;
            }

            return true;
        }
    }
}
