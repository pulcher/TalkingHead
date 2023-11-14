
using Magic8HeadService;
using Magic8HeadService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

public class ReadLcCommand : ICommandMbhToTwitch
{
    private readonly ITwitchClient client;
    private readonly IConfiguration config;
    private readonly IMessageStackService messageStackService;
    private readonly ISayingResponse sayingResponse;
    private readonly IMessageChecker messageChecker;
    private readonly ICommandTracker commandTracker;
    private readonly ILogger<Worker> logger;

    public string Name => "readlc";

    public ReadLcCommand(ITwitchClient client, IConfiguration config, IMessageStackService messageStackService,
        ISayingResponse sayingResponse, IMessageChecker messageChecker, ICommandTracker commandTracker,
        ILogger<Worker> logger)
    {
        this.client = client;
        this.config = config;
        this.messageStackService = messageStackService;
        this.sayingResponse = sayingResponse;
        this.messageChecker = messageChecker;
        this.commandTracker = commandTracker;
        this.logger = logger;
    }

    public void Handle(OnChatCommandReceivedArgs args)
    {
        var lastMessage = messageStackService.GetNextMessage();

        if (lastMessage is not null)
        {
            if (lastMessage.IsSubscriber
            || lastMessage.IsVip
            || lastMessage.IsModerator)
            {
                var username = lastMessage.Username;
                var channel = lastMessage.Channel;

                var commandTrackerEntity = commandTracker.Add(username, "readlc");

                var message = $"Speaking for {username}: who typed {messageChecker.CheckMessage(lastMessage.Message)}";

                sayingResponse.SaySomethingNiceAsync(message, client, channel, username)
                    .Wait();
            }
            else
            {
                client.SendMessage(lastMessage.Channel,
                    $"Hey {lastMessage.Username}, the readlc command is for subscribers and vips only.");
            }
        }
    }
}
