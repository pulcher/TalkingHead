
using Magic8HeadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

public class DiscordCommand : ICommandMbhToTwitch
{
    private readonly ITwitchClient client;
    private readonly IConfiguration config;
    private readonly ILogger<Worker> logger;

    public string Name => "discord";

    public DiscordCommand(TwitchClient client, IConfiguration config, ILogger<Worker> logger)
    {
        this.client = client;
        this.config = config;
        this.logger = logger;
    }

    public void Handle(OnChatCommandReceivedArgs args)
    {
        client.SendMessage(args.Command.ChatMessage.Channel, "https://discord.gg/4X6SSpJNEW");
    }
}
