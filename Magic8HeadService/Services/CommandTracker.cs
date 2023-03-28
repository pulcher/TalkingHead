using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Magic8HeadService.Services
{
    public class CommandTracker : ICommandTracker
    {
        private List<CommandTrackerEntry> trackedCommands = new();

        public void Add(string username, string commandCalled, string commandDetails)
        {
            var entry = new CommandTrackerEntry
            {
                Created = DateTime.UtcNow, 
                Username = username,
                CommandCalled = commandCalled,
                CommandDetails = commandDetails
            };

            trackedCommands.Add(entry);
        }

        public List<CommandTrackerEntry> GetSessionCommands(string command)
        {
            return trackedCommands;
        }
    }
}
