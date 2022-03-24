using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using System;

namespace Magic8HeadService
{
    public class HelpCommandReal : ICommandMbhToTwitch
    {
        private readonly ILogger<Worker> logger;
        private readonly TwitchClient client;

        public string Name => "help";

        // public HelpCommandReal()
        // {
        //     // this.client = client;
        //     // this.logger = logger;
        // }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            // var message = GetHelpMessage();
            Console.WriteLine("-------------- real help here ------------");

            // client.SendMessage(cmd.Command.ChatMessage.Channel, message);
        }

        // private string GetHelpMessage()
        // {
        //     var result = new StringBuilder();

        //     var fields = typeof(AvailableCommands).GetFields();
        //     foreach (var field in fields)
        //     {
        //         var name = field.Name;
        //         var description =
        //             field.CustomAttributes.FirstOrDefault()?.ConstructorArguments.FirstOrDefault().Value;
        //         result.Append($"{name}: {description}, ");
        //     }

        //     var trimmedResult = result.ToString().Substring(0, result.ToString().Length - 2);

        //     trimmedResult += ".  To help code me or request another feature head over to my repository at https://github.com/pulcher/TalkingHead";
        //     return trimmedResult;
        // }
    }
}