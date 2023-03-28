using System.Collections.Generic;

namespace Magic8HeadService.Services
{
    public interface ICommandTracker
    {
        void Add(string username, string commandCalled, string commandDetails = null);
        public List<CommandTrackerEntry> GetSessionCommands(string command);
    }
}
