using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Controller;
using Database;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(static services =>
    {
        services.AddAuthentication();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();
        services.AddScoped<WydDbContext>();
        services.AddScoped<AuthService>();
        services.AddTransient<EventService>();
        services.AddTransient<AccountService>();
        services.AddTransient<UserService>();
        services.AddTransient<ProfileService>();
        services.AddTransient<UtilService>();

        services.AddSingleton(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    })
    .Build();


host.Run();
