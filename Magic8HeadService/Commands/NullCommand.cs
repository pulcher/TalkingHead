using Microsoft.Extensions.Logging;
using TwitchLib.Client.Events;

namespace Magic8HeadService
{
    public class NullCommand : IMbhCommand
    {
        private ILogger<Worker> logger;

        public NullCommand(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        public bool CanExecute()
        {
            return false;
        }

        public void Handle(OnChatCommandReceivedArgs cmd)
        {
            logger.LogInformation($"Nope.... not gonna do it!");
        }
    }
}