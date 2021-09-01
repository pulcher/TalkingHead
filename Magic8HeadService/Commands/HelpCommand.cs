using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Magic8HeadService
{
    public class HelpCommand : IMbhCommand
    {
        private ILogger<Worker> logger;

        public HelpCommand(ILogger<Worker> logger)
        {
            this.logger = logger;    
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            logger.LogInformation($"I will help!");   
        }
    }
}