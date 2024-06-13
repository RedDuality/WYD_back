
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Azure.Security.KeyVault.Secrets;
using System.Text.RegularExpressions;

namespace Database;
public class WydDbContext : DbContext
{


    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<Community> Communities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

/*
        const string secretName = "mySecret";
        var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME") == null ? "wyddbkey" :  Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        var secret = await client.GetSecretAsync("");
*/
        //Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wydreldbserver.database.windows.net,1433;Initial Catalog=Wydreldb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        Console.WriteLine(Environment.GetEnvironmentVariable("SqlConnectionString"));
        
        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasMany(u => u.Events).WithMany(e => e.Users).UsingEntity<UserEvent>();
        modelBuilder.Entity<User>(). HasMany(u => u.Communities).WithMany(c => c.Users);
    }
}