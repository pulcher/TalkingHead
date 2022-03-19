using System;
using System.ComponentModel;

namespace Magic8HeadService
{
    public class ActionCommands
    {
        [Description("Talk to the MBH directly")]
        public const string Mbh = "mbh";
 
        [Description("Time the current stream has been running.")]
        public const string Uptime = "uptime";

        [Description("Current commands that have actions.")]
        public const string HelpCommand = "help";
    }
}