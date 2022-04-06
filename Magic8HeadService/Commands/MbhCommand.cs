
using System.Collections.Generic;
using System.Linq;
using Magic8HeadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;

public class MbhCommand : ICommandMbhToTwitch
{
    private TwitchClient client;
    private readonly IConfiguration config;
    private readonly ISayingResponse sayingResponse;
    private readonly IDadJokeService dadJokeService;
    private ILogger<Worker> logger;
    private IMbhCommand action;
    private string mood= Moods.Snarky;

    public string Name => "mbh";

    public MbhCommand(TwitchClient client, IConfiguration config, ISayingResponse sayingResponse,
        IDadJokeService dadJokeService, ILogger<Worker> logger)
    {
        this.client         = client;
        this.config         = config;
        this.sayingResponse = sayingResponse;
        this.dadJokeService = dadJokeService;
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
                action = new SayCommand(client, sayingResponse, logger);
                break;
            default:
                break;
        }

        action.Handle(args);
        return;
    }
}
