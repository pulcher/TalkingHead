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
        conf.AddUserSecrets<Program>(optional: true, reloadOnChange: false);
    })
    .Build();

host.Run();
