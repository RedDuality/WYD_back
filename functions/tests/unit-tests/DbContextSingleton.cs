using System;
using Database;

public class DbContextSingleton
{
    private static WydDbContext? _instance;
    private static readonly object _lock = new object();

    // Private constructor to prevent instantiation
    private DbContextSingleton()
    {
    }

    // Public static method to get the singleton instance
    public static WydDbContext Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) // Ensure thread safety
                {
                    if (_instance == null)
                    {
                        Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddatabaseserver.database.windows.net,1433;Initial Catalog=wydtestdb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

                        _instance = new WydDbContext();
                    }
                }
            }

            return _instance;
        }
    }
}
