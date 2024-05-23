using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Controller;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(static services => {
        services.AddAuthentication();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.AddTransient<AuthController>();
    })
    .Build();
    
JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
     Formatting = Formatting.Indented,
     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};

host.Run();
