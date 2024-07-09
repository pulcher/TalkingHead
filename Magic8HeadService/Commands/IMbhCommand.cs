using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public interface IMbhCommand
    {
        bool CanExecute();
        void Handle(OnChatCommandReceivedArgs cmd);
    }
}
