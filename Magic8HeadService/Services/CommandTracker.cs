using System;
using System.Collections.Generic;
using MrBigHead.Shared;

namespace Magic8HeadService.Services
{
    public class CommandTracker : ICommandTracker
    {
        private List<CommandTrackerEntry> trackedCommands = new();

        public CommandTrackerEntry Add(string username, string commandCalled, string  commandDetails)
        {
            var entry = new CommandTrackerEntry
            {
                Created = DateTime.UtcNow, 
                Username = username,
                CommandCalled = commandCalled,
                CommandDetails = commandDetails
            };

            trackedCommands.Add(entry);

            return entry;
        }

        public List<CommandTrackerEntry> GetSessionCommands(string command)
        {
            return trackedCommands;
        }
    }
}
