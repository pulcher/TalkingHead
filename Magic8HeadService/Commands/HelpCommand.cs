using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public class HelpCommand : IMbhCommand
    {
        private readonly ILogger<Worker> logger;
        private readonly ITwitchClient client;

        public string Name => "help";

        public HelpCommand(ITwitchClient client, ILogger<Worker> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            var message = GetHelpMessage();

            client.SendMessage(cmd.Command.ChatMessage.Channel, message);
        }

        private string GetHelpMessage()
        {
            var result = new StringBuilder();

            var fields = typeof(AvailableCommands).GetFields();
            foreach (var field in fields)
            {
                var name = field.Name;
                var description =
                    field.CustomAttributes.FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value;
                result.Append($"{name}: {description}, ");
            }

            var trimmedResult = result.ToString().Substring(0, result.ToString().Length - 2);

            trimmedResult += ".  To help code me or request another feature head over to my repository at https://github.com/pulcher/TalkingHead";
            return trimmedResult;
        }
    }
}