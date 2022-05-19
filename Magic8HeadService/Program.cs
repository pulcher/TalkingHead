using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrBigHead.Services;
using Scrutor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Magic8HeadService.Options;

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
          services.Configure<TwitchBotCredentials>(hostContext.Configuration.GetSection("TwitchBotConfiguration"));
          services.Configure<SpeechConfiguration>(hostContext.Configuration.GetSection("SpeechService"));
          services.Configure<VoiceConfiguration>(hostContext.Configuration.GetSection("Voice"));

          services.AddSingleton<ConnectionCredentials>((svc) =>
          {
            var val = svc.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<TwitchBotCredentials>>().Value.AsConnectionCredentials();

            return val;
          });


          services.AddSingleton<ClientOptions>( svc =>
          {
            var clientOption = new ClientOptions{
            DisconnectWait = 5000,
            MessagesAllowedInPeriod = 750,
            ReconnectionPolicy = new ReconnectionPolicy(3000, maxAttempts: 50),
            ThrottlingPeriod = TimeSpan.FromSeconds(30),
            UseSsl = true};

            return clientOption;
          });

          services.AddSingleton<WebSocketClient>();
          services.AddSingleton<TwitchClient>();

          services.AddOptions<List<string>>("CommandNames").Configure<IServiceProvider>((str, svc) =>
          {

            var commandNames = svc.GetServices<ICommandMbhToTwitch>()
//              .Where(x => x.GetType() == typeof())
              .Select(cmd => cmd.Name);
            str.AddRange(commandNames);
          });


          services.AddSingleton<IApiSettings>(provider =>
          {
            var credentials = provider.CreateScope().ServiceProvider.GetRequiredService<IOptionsSnapshot<TwitchBotCredentials>>().Value;
            return new ApiSettings
            {
              ClientId = credentials.ClientID,
              AccessToken = credentials.AccessToken,
            };
          });

          services.AddSingleton<TwitchAPI>();

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
        })
      
      
      ;
  }

}
