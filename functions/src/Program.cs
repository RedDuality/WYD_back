using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Controller;
using Microsoft.Extensions.Configuration;
using Database;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(static services => {
        services.AddAuthentication();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.AddScoped<WydDbContext>();
        services.AddTransient<AuthController>();
        services.AddTransient<EventController>();
        services.AddTransient<UserController>();
        services.AddTransient<CommunityController>();
    })
    .Build();
    
JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
     Formatting = Formatting.Indented,
     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};

host.Run();
