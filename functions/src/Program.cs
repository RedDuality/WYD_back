using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Service;
using Database;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(static services =>
    {
        services.AddAuthentication();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging();

        services.AddDbContext<WydDbContext>(options =>
        {
            Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddatabaseserver.database.windows.net,1433;Initial Catalog=wyddb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not set.");
            }

            options.UseSqlServer(connectionString)
                   .UseLazyLoadingProxies();
        });
        services.AddTransient<SignalRService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddTransient<AuthorizationService>();
        services.AddTransient<RequestService>();

        services.AddTransient<AccountService>();
        services.AddTransient<UserService>();
        services.AddTransient<ProfileService>();
        services.AddTransient<CommunityService>();
        services.AddTransient<GroupService>();
        services.AddTransient<EventService>();

        services.AddTransient<UtilService>();

        services.AddSingleton(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    })
    .Build();


host.Run();
