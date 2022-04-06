using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrBigHead.Services;
using Scrutor;
using System;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Magic8HeadService
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddUserSecrets<Program>(optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    
                    var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

                    var userName = configuration["TwitchBotConfiguration:UserName"];
                    var accessToken = configuration["TwitchBotConfiguration:AccessToken"];
                    var credentials = new ConnectionCredentials(userName, accessToken);

                    services.AddSingleton(credentials);

                    services.AddSingleton(new ClientOptions
                        {
                            MessagesAllowedInPeriod = 750,
                            ThrottlingPeriod = TimeSpan.FromSeconds(30),
                            UseSsl = true
                        });

                    services.AddSingleton<WebSocketClient>();
                    services.AddSingleton<TwitchClient>();

                    services.AddHttpClient();

                    // adding the ICommandMbhToTwitch main series of commands
                    services.Scan(scan => scan
                        .FromAssemblyOf<ICommandMbhToTwitch>()
                            .AddClasses(classes => classes.AssignableTo<ICommandMbhToTwitch>())
                                .AsImplementedInterfaces()
                                .WithTransientLifetime());

                    services.AddScoped<ISayingService, SayingService>();
                    services.AddScoped<ISayingResponse, SayingResponse>();
                    services.AddScoped<IDadJokeService, DadJokeService>();
                    services.AddScoped<ICommandMbhTwitchHelp, HelpCommandReal>();

                    services.AddHostedService<Worker>();

                    Console.WriteLine($"services has a count: {services.Count}");
                });
    }
}


/*
            var credentials = new ConnectionCredentials(userName, accessToken);
	        var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30),
                    UseSsl = true
                };
            var customClient = new WebSocketClient(clientOptions);


            client = new TwitchClient(customClient);
            client.Initialize(credentials, "haroldpulcher");
*/