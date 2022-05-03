using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadServiceTests;

[TestClass]
public class DiscordCommandTest
{
    private IConfiguration configuration;
    private ILogger<Magic8HeadService.Worker> logger;
    private ITwitchClient twitchClient;

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

        twitchClient = new TwitchClient();
    }

    [TestMethod]
    public void CanInstance()
    {
        var command = new DiscordCommand(twitchClient, configuration, logger);
    }
}
