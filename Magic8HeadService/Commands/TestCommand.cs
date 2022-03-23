using System;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public class TestCommand : ITwitchCommand
    {
        public string Name { get => "test";}
 
        public void Handle(OnChatCommandReceivedArgs args)
        {
            Console.WriteLine("****** testing the ITwitch Command *****");

            return;
        }
    }
}
