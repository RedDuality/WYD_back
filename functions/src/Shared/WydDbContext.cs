using Microsoft.EntityFrameworkCore;
using Model;

namespace Database;
public class WydDbContext : DbContext
{


    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<Community> Communities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        //Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddbserver.database.windows.net,1433;Initial Catalog=WYD-p-db;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        //Console.WriteLine("ConnectionString "+ Environment.GetEnvironmentVariable("SqlConnectionString"));
        
        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasMany(u => u.Events).WithMany(e => e.Users).UsingEntity<UserEvent>();
        modelBuilder.Entity<User>(). HasMany(u => u.Communities).WithMany(c => c.Users);
    }
}