using Microsoft.EntityFrameworkCore;
using Model;

namespace Database;
public class WydDbContext : DbContext
{
    //Main Entity
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;

    //joins
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<UserGroup> UserGroups { get; set; } = null!;
    public DbSet<ProfileEvent> ProfileEvents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

//          Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddatabaseserver.database.windows.net,1433;Initial Catalog=wyddb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
//TEST        
//            Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddatabaseserver.database.windows.net,1433;Initial Catalog=wydtestdb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>().HasMany(u => u.Accounts);
        modelBuilder.Entity<User>().HasIndex(u => u.Tag).IsUnique().HasFilter("Tag <> ''");
        modelBuilder.Entity<User>().HasOne(u => u.MainProfile);
        modelBuilder.Entity<User>().HasMany(u => u.Profiles).WithMany(p => p.Users).UsingEntity<UserRole>();
        modelBuilder.Entity<User>().HasMany(u => u.Groups).WithMany(g => g.Users).UsingEntity<UserGroup>();
        modelBuilder.Entity<Profile>().HasMany(p => p.Events).WithMany(e => e.Profiles).UsingEntity<ProfileEvent>();

    }


    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity);

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.Now;
                entity.UpdatedAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }


}