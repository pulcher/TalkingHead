using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Magic8HeadService;

namespace Magic8HeadServiceTests;

[TestClass]
public class MessageCheckerTest
{
    private ILogger<Worker> logger;


    [TestInitialize]
    public void Setup() 
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetService<ILoggerFactory>();

        logger = factory.CreateLogger<Magic8HeadService.Worker>();
    }

    [TestMethod]
    public void CanInstance()
    {
        var command = new MessageChecker(logger);
        Assert.IsNotNull(command);
    }
    
    [TestMethod]
    public void CheckMessagePlainText()
    {
        var command = new MessageChecker(logger);
        var plainText = "This is a plain text message";
        Assert.AreEqual(plainText, command.CheckMessage(plainText));
    }

    [TestMethod]
    public void CheckMessageWithLink()
    {
        var command = new MessageChecker(logger);
        var messageWithLink = "This is my message with http://www.mylink.com/";
        Assert.AreEqual("This is my message with link", command.CheckMessage(messageWithLink));
    }

    [TestMethod]
    public void CheckMessageWithJustWWW()
    {
        var command = new MessageChecker(logger);
        var messageWithWWW = "This is my message for www.mylink.com";
        Assert.AreEqual(messageWithWWW, command.CheckMessage(messageWithWWW));
    }
}