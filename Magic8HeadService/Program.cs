using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MrBigHead.Services;

namespace Magic8HeadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                    services.AddScoped<ISayingService, SayingService>();
                    services.AddScoped<ISayingResponse, SayingResponse>();
                    services.AddHostedService<Worker>();
                })
                .;
    }
}
