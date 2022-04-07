using System;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public class TestCommand : ICommandMbhToTwitch
    {
        public string Name { get => "test";}
 
        public void Handle(OnChatCommandReceivedArgs args)
        {
            Console.WriteLine("****** testing the ICommandMBHtoTwitch interface *****");

            return;
        }
    }
}
