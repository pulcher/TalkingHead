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
using TwitchLib.Client.Internal.Parsing;

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

        Assert.IsNotNull(command);
    }

    [TestMethod]
    [Ignore]
    public void GivenCommandArgsMessageSent()
    {
        // assemble
        // _mockClient.ReceiveMessage($"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");
        //var ircParser = new IrcParser();

        // this is a public method that gets new'ed up inside the client.  
        // probably just need to mock it's output.  Not sure how to get that ATM.
        // added the InternalsVisibleTo to the project file.  Still not helping.
        // ignoring this test for the moment.
        // var ircParsedMessage = ircParser.ParseIrcMessage($"@badges=subscriber/0,premium/1;color=#005C0B;display-name=KIJUI;emotes=30259:0-6;id=fefffeeb-1e87-4adf-9912-ca371a18cbfd;mod=0;room-id=22510310;subscriber=1;tmi-sent-ts=1530128909202;turbo=0;user-id=25517628;user-type= :kijui!kijui@kijui.tmi.twitch.tv PRIVMSG #testchannel :TEST MESSAGE");

        var ircParameters = new string[] {};
        var ircMessage = new IrcMessage(IrcCommand.Unknown, ircParameters, "!theUser", new Dictionary<string, string>());

        var messageEmoteCollection = new MessageEmoteCollection();

        var chatMessage = new ChatMessage("theBot", ircMessage, ref messageEmoteCollection, false);
        var chatCommand = new ChatCommand(chatMessage);

        var commandEventArgs = new OnChatCommandReceivedArgs
        {
            Command = chatCommand
        };

        var command = new DiscordCommand(twitchFake, configuration, logger);

        // act
        command.Handle(commandEventArgs);

        // assert
        A.CallTo(() => twitchFake.SendMessage(A<string>.Ignored, A<string>.Ignored, A<bool>.Ignored)).MustHaveHappened();
    }
}
