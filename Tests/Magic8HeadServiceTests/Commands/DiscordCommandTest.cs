using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client.Interfaces;
using FakeItEasy;
using TwitchLib.Client.Enums.Internal;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Internal;

namespace Magic8HeadServiceTests;

[TestClass]
public class DiscordCommandTest
{
    private IConfiguration configuration;
    private ILogger<Magic8HeadService.Worker> logger;
    private ITwitchClient twitchFake;

    [TestInitialize]
    public void Setup() 
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"TopLevelKey", "TopLevelValue"},
            {"SectionName:SomeKey", "SectionValue"},
            //...populate as needed for the test
        };

        configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        logger = factory.CreateLogger<Magic8HeadService.Worker>();

        twitchFake = A.Fake<ITwitchClient>();
    }

    [TestMethod]
    public void CanInstance()
    {
        var command = new DiscordCommand(twitchFake, configuration, logger);
    }

    [TestMethod]
    public void GivenCommandArgsMessageSent()
    {
        // assemble
        var ircParameters = new string[] {};
        var ircMessage = new IrcMessage(IrcCommand.Unknown, ircParameters, "theUser", null);

        var messageEmoteCollection = new MessageEmoteCollection();
        var commandEventArgs = new OnChatCommandReceivedArgs
        {
            Command = new ChatCommand(new ChatMessage("theBot", ircMessage, ref messageEmoteCollection, false))
        };

        var command = new DiscordCommand(twitchFake, configuration, logger);

        // act
        command.Handle(commandEventArgs);

        // assert
        A.CallTo(() => twitchFake.SendMessage(A<string>.Ignored, A<string>.Ignored, A<bool>.Ignored)).MustHaveHappened();
    }
}
