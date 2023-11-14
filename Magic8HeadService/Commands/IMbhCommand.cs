using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public interface IMbhCommand
    {
        void Handle(OnChatCommandReceivedArgs cmd);
    }
}
