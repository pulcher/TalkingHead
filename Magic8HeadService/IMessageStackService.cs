using TwitchLib.Client.Models;

namespace Magic8HeadService
{
    public interface IMessageStackService
    {
        ChatMessage GetNextMessage();
        ChatMessage GetPreviousMessage(int previousIndex);
        void PutMessage(ChatMessage message);
    }
}
