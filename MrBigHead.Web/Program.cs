using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MrBigHead.Web.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MrBigHead.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<UserInformationProvider, UserInformationProvider>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
            });

            builder.Services.AddLogging(options =>
            {
                builder.Configuration.Bind("Logging");
            });

            await builder.Build().RunAsync();
        }
    }
}
