using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Magic8HeadService
{
    public class HelpCommand : IMbhCommand
    {
        private ILogger<Worker> logger;
        private TwitchClient client;

        public HelpCommand(TwitchClient client, ILogger<Worker> logger)
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