using System;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public interface ICommandMbhToTwitch
    {
        public string Name { get; }
        public void Handle(OnChatCommandReceivedArgs args);
    }
}
