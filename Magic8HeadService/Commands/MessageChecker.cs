using System;

namespace Magic8HeadService
{
    public class MessageChecker : IMessageChecker
    {
        public string CheckMessage(string message)
        {
            // Check if there's a URI inside the message
            string[] array = message.Split(' ');
            for (int i = 0; i < array.Length; i++)
            {
                string word = array[i];
                if (Uri.IsWellFormedUriString(word, UriKind.RelativeOrAbsolute))
                    array[i] = "link";
            }
            return String.Join(' ', array);
        }
    }
}