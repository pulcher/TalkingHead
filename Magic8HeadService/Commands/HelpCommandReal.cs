using TwitchLib.Client.Events;
using System;

namespace Magic8HeadService
{
    public class HelpCommandReal : ICommandMbhToTwitch, ICommandMbhTwitchHelp
    {
        public string Name => "help";

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            Console.WriteLine("-------------- real help here ------------");
        }
    }
}