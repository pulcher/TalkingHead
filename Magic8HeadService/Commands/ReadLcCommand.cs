
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
        var peekMessage = messageStackService.PeekNextMessage();
        //var lastMessage = messageStackService.GetNextMessage();

        if (peekMessage is not null)
        {
            if (args.Command.ChatMessage.IsSubscriber
            || args.Command.ChatMessage.IsVip
            || args.Command.ChatMessage.IsModerator)
            {
                var username = peekMessage.Username;
                var channel = peekMessage.Channel;

                var commandTrackerEntity = commandTracker.Add(username, "readlc");

                var message = $"Speaking for {username}: who typed {messageChecker.CheckMessage(peekMessage.Message)}";

                sayingResponse.SaySomethingNiceAsync(message, client, channel, null)
                    .Wait();

                messageStackService.GetNextMessage();
            }
            else
            {
                client.SendMessage(peekMessage.Channel,
                    $"Hey {peekMessage.Username}, subscribe now for the readlc command as well as many other benefits!");
            }
        }
    }
}
