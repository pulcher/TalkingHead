using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    //.ConfigureLogging(logging =>
    //{
    //    logging.SetMinimumLevel(LogLevel.Debug);
    //})
    .Build();

host.Run();
