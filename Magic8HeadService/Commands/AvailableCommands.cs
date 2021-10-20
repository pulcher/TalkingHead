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
        [Description("Ask Mr. Big Head to say something... he may choose not to say it.")]
        public const string Say = "say";
        [Description("Be inspired by an inspiring quote!")]
        public const string Inspire = "inspire";
        [Description("Tell a Dad joke")]
        public const string Dad = "dad";

        // [Description("Just an old fashions shoutout!")]
        // public const string Shoutout = "so";
    }
}