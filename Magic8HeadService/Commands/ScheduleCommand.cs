
using System.Threading.Tasks;
using Magic8HeadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;

public class ScheduleCommand : ICommandMbhToTwitch
{
    private readonly TwitchClient client;
    private readonly TwitchAPI api;
    private readonly IConfiguration config;
    private readonly ILogger<Worker> logger;

    public string Name => "schedule";

    public ScheduleCommand(TwitchClient client, TwitchAPI api, IConfiguration config, ILogger<Worker> logger)
    {
        this.client = client;
        this.api    = api;
        this.config = config;
        this.logger = logger;
    }

    public async void Handle(OnChatCommandReceivedArgs args)
    {
        logger.LogInformation($"handling the Schedule command");

        var schedule = await api.Helix.Schedule.GetChannelStreamScheduleAsync("211523303");

        var nextStream = $"next stream: {schedule.Schedule.Segments[0].Title} Starts at {schedule.Schedule.Segments[0].StartTime.ToString("g")} UTC";
        logger.LogInformation(nextStream);

        client.SendMessage(args.Command.ChatMessage.Channel, nextStream);
    }
}
