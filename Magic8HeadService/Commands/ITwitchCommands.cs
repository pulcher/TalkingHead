using System;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public interface ITwitchCommand
    {
        public string Name { get; }
        public void Handle(OnChatCommandReceivedArgs args);
    }
}
