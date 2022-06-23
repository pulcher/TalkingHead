using System;
using Microsoft.Extensions.Logging;

namespace Magic8HeadService
{
    public class MessageChecker : IMessageChecker
    {
        private readonly ILogger<Worker> logger;
        public MessageChecker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        public string CheckMessage(string message)
        {
            // Check if there's a URI inside the message
            string[] array = message.Split(' ');
            for (int i = 0; i < array.Length; i++)
            {
                string word = array[i];
                if (Uri.TryCreate(word, UriKind.RelativeOrAbsolute, out var uriResult) && uriResult.IsAbsoluteUri)
                {
                    array[i] = "link";
                }
    
            }
            return String.Join(' ', array);
        }
    }
}