using TwitchLib.Client.Models;

namespace Magic8HeadService
{
    public interface IMessageStackService
    {
        ChatMessage GetNextMessage();
        ChatMessage GetPreviousMessage(int previousIndex);
        ChatMessage PeekNextMessage();
        void PutMessage(ChatMessage message);
    }
}
