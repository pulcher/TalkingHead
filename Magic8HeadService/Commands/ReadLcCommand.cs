
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
        if (args.Command.ChatMessage.IsSubscriber
            || args.Command.ChatMessage.IsVip
            || args.Command.ChatMessage.IsModerator)
        {
            var peekMessage = messageStackService.PeekNextMessage();

            if (peekMessage is not null)
            {
                var lastMessage = messageStackService.GetNextMessage();

                var commandTrackerEntity = commandTracker.Add(args.Command.ChatMessage.Username, "readlc");

                var message = $"Speaking for {lastMessage.Username}: who typed {messageChecker.CheckMessage(lastMessage.Message)}";

                sayingResponse.SaySomethingNiceAsync(message, client, lastMessage.Channel, null)
                    .Wait();
            }
            else
            {
                var message = $"Sorry {args.Command.ChatMessage.Username}, I don't have anything else interesting to repeat.";
                sayingResponse.SaySomethingNiceAsync(message, client, args.Command.ChatMessage.Channel, null)
                    .Wait();
            }
        }
        else
        {
            client.SendMessage(args.Command.ChatMessage.Channel,
                $"Hey {args.Command.ChatMessage.Username}, subscribe now for the readlc command as well as many other benefits!");

        }
    }
}
