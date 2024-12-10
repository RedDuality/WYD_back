using Microsoft.EntityFrameworkCore;
using Model;

namespace Database;
public class WydDbContext(DbContextOptions<WydDbContext> options) : DbContext(options)
{

    //Main Entity
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Profile> Profiles { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Blob> Blobs { get; set; } = null!;
    public DbSet<Community> Communities { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;

    //joins
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<ProfileCommunity> UserCommunities { get; set; } = null!;
    public DbSet<ProfileGroup> UserGroups { get; set; } = null!;
    public DbSet<ProfileEvent> ProfileEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<User>().HasMany(u => u.Accounts);

        modelBuilder.Entity<User>().HasOne(u => u.MainProfile);
        modelBuilder.Entity<User>().HasMany(u => u.Profiles).WithMany(p => p.Users).UsingEntity<UserRole>();

        modelBuilder.Entity<Profile>().HasIndex(u => u.Tag).IsUnique().HasFilter("Tag <> ''");
        modelBuilder.Entity<Profile>().HasMany(p => p.Events).WithMany(e => e.Profiles).UsingEntity<ProfileEvent>();
        modelBuilder.Entity<Profile>().HasMany(u => u.Communities).WithMany(c => c.Profiles).UsingEntity<ProfileCommunity>();
        modelBuilder.Entity<Profile>().HasMany(u => u.Groups).WithMany(g => g.Profiles).UsingEntity<ProfileGroup>();

        modelBuilder.Entity<Event>().HasMany(e => e.Blobs);

        modelBuilder.Entity<Community>().HasMany(c => c.Groups).WithOne(g => g.Community);



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