using System;
using System.Collections.Generic;
using Magic8HeadService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

public class UptimeCommand : ICommandMbhToTwitch
{
    private readonly ITwitchClient client;
    private readonly ITwitchAPI api;
    private readonly IConfiguration config;
    private readonly ILogger<Worker> logger;

    public string Name => "uptime";


    public UptimeCommand(ITwitchClient client, ITwitchAPI api, IConfiguration config, ILogger<Worker> logger)
    {
        this.client = client;
        this.api = api;
        this.config = config;
        this.logger = logger;
    }

    public async void Handle(OnChatCommandReceivedArgs args)
    {
        var userIds = new List<string> { "211523303"};

        var streamStuff = await api.Helix.Streams.GetStreamsAsync(userIds: userIds);

        var startedAtDate = streamStuff.Streams[0].StartedAt;

        var currentDate = DateTime.UtcNow;

        var uptime = currentDate - startedAtDate;

        client.SendMessage(args.Command.ChatMessage.Channel, 
            $"The stream has been up for {uptime.ToString("hh")} hours, {uptime.ToString("mm")} minutes, and {uptime.ToString("ss")} seconds. But who's counting?");
    }
}
