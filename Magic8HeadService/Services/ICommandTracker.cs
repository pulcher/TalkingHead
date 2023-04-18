using MrBigHead.Shared;
using System.Collections.Generic;

namespace Magic8HeadService.Services
{
    public interface ICommandTracker
    {
        public CommandTrackerEntry Add(string username, string commandCalled, string commandDetails = null);
        public List<CommandTrackerEntry> GetSessionCommands(string command);
    }
}
