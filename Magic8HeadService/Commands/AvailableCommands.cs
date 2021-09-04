using System;
using System.ComponentModel;

namespace Magic8HeadService
{
    public class AvailableCommands
    {
        [Description("All of the commands")]
        public const string Help = "help";
        [Description("Ask your burning question and get a response")]
        public const string Ask = "ask";
        // [Description("Just an old fashions shoutout!")]
        // public const string Shoutout = "so";
    }
}