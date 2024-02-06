using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    //.ConfigureLogging(logging =>
    //{
    //    logging.SetMinimumLevel(LogLevel.Debug);
    //})
    .ConfigureAppConfiguration( conf =>
    {
        //var credential = new DefaultAzureCredential();

        //config.AddAzureKeyVault(new System.Uri("https://mykv.vault.azure.net/"), credential);
        conf.AddUserSecrets<Program>(optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
