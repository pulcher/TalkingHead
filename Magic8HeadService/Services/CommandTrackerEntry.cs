using System;

namespace Magic8HeadService.Services
{
    public class CommandTrackerEntry
    {
        public DateTime Created { get; set; }
        public string Username { get; set; }
        public string CommandCalled { get; set; }
        public string CommandDetails { get; set; }
    }
}
