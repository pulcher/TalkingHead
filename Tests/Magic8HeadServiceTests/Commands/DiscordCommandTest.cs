using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client.Interfaces;
using FakeItEasy;
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
        var ircMessage = new IrcMessage("theUser");

        var commandEventArgs = new OnChatCommandReceivedArgs();
        commandEventArgs.Command = new Chatcommand(new ChatMessage("theBot", ircMessage, ref new MessageEmoteCollection(), false));

        var command = new DiscordCommand(twitchFake, configuration, logger);

        // act
        command.Handle(commandEventArgs);

        // assert
        A.CallTo(() => twitchFake.SendMessage(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
    }
}
