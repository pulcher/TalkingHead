using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrBigHead.Services;
using Scrutor;
using System;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
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
                    // Yes, we are using this in production as well.  
                    // we will change this eventually
                    builder.AddUserSecrets<Program>(optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    
                    var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

                    // probably could convert this into a bind and only need to pass around this object
                    // well, I believe Huga may have a better idea. :) aka IOptions<T>
                    var twitchBotConfiguration = new TwitchBotConfiguration();
                    configuration.GetSection("TwitchBotConfiguration").Bind(twitchBotConfiguration);

                    services.AddSingleton(twitchBotConfiguration);

                    var credentials = new ConnectionCredentials(twitchBotConfiguration.UserName, twitchBotConfiguration.AccessToken);
                    services.AddSingleton(credentials);

                    services.AddSingleton(new ClientOptions
                        {
                            DisconnectWait = 5000,
                            MessagesAllowedInPeriod = 750,
                            ReconnectionPolicy = new ReconnectionPolicy(3000, maxAttempts: 50),
                            ThrottlingPeriod = TimeSpan.FromSeconds(30),
                            UseSsl = true
                        });

                    services.AddSingleton<WebSocketClient>();

                    // Setup access to the API and register it.
                    services.AddSingleton<IApiSettings>(new ApiSettings
                        { 
                            ClientId = twitchBotConfiguration.ClientId,
                            AccessToken = twitchBotConfiguration.AccessToken
                        });

                    //services.AddSingleton<TwitchAPI>();
                    services.AddSingleton<ITwitchAPI, TwitchAPI>();

                    services.AddHttpClient();

                    //services.AddSingleton<TwitchClient>();
                    services.AddSingleton<ITwitchClient, TwitchClient>();

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
