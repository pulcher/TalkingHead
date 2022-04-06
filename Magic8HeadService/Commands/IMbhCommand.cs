using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Magic8HeadService
{
    public interface IMbhCommand
    {
        void Handle(OnChatCommandReceivedArgs cmd);
    }
}
