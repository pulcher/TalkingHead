using System.Collections.Generic;
using TwitchLib.Client.Models;

namespace Magic8HeadService
{
    public class MessageStackService : IMessageStackService
    {
        private readonly Stack<ChatMessage> messageStack;

        public MessageStackService()
        {
            messageStack = new Stack<ChatMessage>(30);
        }
        public ChatMessage GetNextMessage()
        {
            messageStack.TryPop(out var result);
            return result;
        }

        public ChatMessage GetPreviousMessage(int previousIndex)
        {
            throw new System.NotImplementedException();
        }

        public ChatMessage PeekNextMessage()
        {
            messageStack.TryPeek(out var result);
            return result;
        }

        public void PutMessage(ChatMessage message)
        {
            if (message == null) return;

            messageStack.TrimExcess();
            messageStack.Push(message);
        }
    }
}
