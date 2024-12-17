using BlobFunction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })

    //This is required for the Azurite Local Emulator to work correctly.
    .ConfigureAppConfiguration(config => { config.AddUserSecrets<Function1>(optional: true, reloadOnChange: false); })

    .Build();


host.Run();
