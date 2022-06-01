
using Magic8HeadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

public class GiveawayCommand : ICommandMbhToTwitch
{
    private readonly ITwitchClient client;
    private readonly IConfiguration config;
    private readonly ILogger<Worker> logger;

    public string Name => "giveaway";

    public GiveawayCommand(ITwitchClient client, IConfiguration config, ILogger<Worker> logger)
    {
        this.client = client;
        this.config = config;
        this.logger = logger;
    }

    public void Handle(OnChatCommandReceivedArgs args)
    {
        client.SendMessage(args.Command.ChatMessage.Channel, 
            "Win a $100 Amazon Gift Card.  Register at https://contests.davidriewe.com/harold-pulcher-give-away-4-22/");
    }
}
