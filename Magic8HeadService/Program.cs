using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrBigHead.Services;
using Scrutor;
using System;
using System.Threading.Tasks;

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
                    
                    services.AddHttpClient();

                    services.Scan(scan => scan
                        // We start out with all types in the assembly of ICommandMbhToTwitch
                        .FromAssemblyOf<ICommandMbhToTwitch>()
                            // AddClasses starts out with all public, non-abstract types in this assembly.
                            // These types are then filtered by the delegate passed to the method.
                            // In this case, we filter out only the classes that are assignable to ITransientService.
                            .AddClasses(classes => classes.AssignableTo<ICommandMbhToTwitch>())
                                // We then specify what type we want to register these classes as.
                                // In this case, we want to register the types as all of its implemented interfaces.
                                // So if a type implements 3 interfaces; A, B, C, we'd end up with three separate registrations.
                                .AsImplementedInterfaces()
                                // And lastly, we specify the lifetime of these registrations.
                                .WithTransientLifetime());

                    services.AddScoped<ISayingService, SayingService>();
                    services.AddScoped<ISayingResponse, SayingResponse>();
                    services.AddScoped<IDadJokeService, DadJokeService>();

                    services.AddHostedService<Worker>();

                    Console.WriteLine($"services has a count: {services.Count}");
                });
    }
}
