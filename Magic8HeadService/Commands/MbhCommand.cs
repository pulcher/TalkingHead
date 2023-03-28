
using System.Collections.Generic;
using System.Linq;
using Magic8HeadService;
using Magic8HeadService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

public class MbhCommand : ICommandMbhToTwitch
{
    private ITwitchClient client;
    private readonly IConfiguration config;
    private readonly ISayingResponse sayingResponse;
    private readonly IDadJokeService dadJokeService;
    private readonly IMessageChecker messageChecker;
    private ILogger<Worker> logger;
    private IMbhCommand action;
    private string mood= Moods.Snarky;
    private ICommandTracker commandTracker;

    public string Name => "mbh";

    public MbhCommand(ITwitchClient client, IConfiguration config, ISayingResponse sayingResponse,
        IDadJokeService dadJokeService, IMessageChecker messageChecker, ICommandTracker commandTracker, ILogger<Worker> logger)
    {
        this.client         = client;
        this.config         = config;
        this.sayingResponse = sayingResponse;
        this.dadJokeService = dadJokeService;
        this.messageChecker = messageChecker;
        this.commandTracker = commandTracker;
        this.logger         = logger;
    }

    public void Handle(OnChatCommandReceivedArgs args)
    {
        action = new NullCommand(logger);

        switch (args.Command.ArgumentsAsList.FirstOrDefault()?.ToLower())
        {
            case AvailableCommands.Help:
                // case null:
                action = new HelpCommand(client, logger);
                break;
            case AvailableCommands.Ask:
                action = new AskCommand(client, sayingResponse, mood, logger);
                break;
            case AvailableCommands.Dad:
                action = new DadCommand(client, sayingResponse, dadJokeService, logger);
                break;
            case AvailableCommands.Inspire:
                action = new InspirationalCommand(client, sayingResponse, logger);
                break;
            case AvailableCommands.Say:
                action = new SayCommand(client, sayingResponse, messageChecker, commandTracker, logger);
                break;
            default:
                break;
        }

        action.Handle(args);
        return;
    }
}
