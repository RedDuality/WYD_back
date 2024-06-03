using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddTransient<AuthService>();
        services.AddTransient<EventService>();
        services.AddTransient<UserService>();
        services.AddTransient<CommunityService>();
    })
    .Build();


host.Run();
