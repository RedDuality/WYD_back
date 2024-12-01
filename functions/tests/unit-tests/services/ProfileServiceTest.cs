
using Service;
using Database;
using Microsoft.EntityFrameworkCore;
using Model;

namespace unit_tests.services;

public class ProfileServiceTest
{
    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new WydDbContext(options);
    }

    [Fact]
    public void Get_ById_ReturnsProfile()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Profiles.Add(new Profile { Id = 1 });
        dbContext.SaveChanges();

        var service = new ProfileService(dbContext);

        // Act
        var result = service.Get(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Create_AddsProfileToDatabase()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new ProfileService(dbContext);
        var profile = new Profile { Id = 2 };

        // Act
        var result = service.Create(profile);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);

        var createdProfile = dbContext.Profiles.Find(2);
        Assert.NotNull(createdProfile);
    }

    [Fact]
    public void GetProfiles_ByIds_ReturnsMatchingProfiles()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Profiles.AddRange(
            new Profile { Id = 1 },
            new Profile { Id = 2 },
            new Profile { Id = 3 }
        );
        dbContext.SaveChanges();

        var service = new ProfileService(dbContext);

        // Act
        var result = service.GetProfiles(new List<int> { 1, 3 });

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == 1);
        Assert.Contains(result, p => p.Id == 3);
    }
    [Fact]
    public void RetrieveEvents_ReturnsEventDtosForProfile()
    {
        // Arrange
        var profile = new Profile
        {
            Id = 1,
            Events = new List<Event>
        {
            new Event
            {
                Hash = "hash1",
                Title = "Event 1",
                Description = "Description 1",
                StartTime = DateTimeOffset.Now,
                EndTime = DateTimeOffset.Now.AddHours(2)
            },
            new Event
            {
                Hash = "hash2",
                Title = "Event 2",
                Description = "Description 2",
                StartTime = DateTimeOffset.Now.AddDays(1),
                EndTime = DateTimeOffset.Now.AddDays(1).AddHours(2)
            }
        }
        };

        // Act
        var result = ProfileService.RetrieveEvents(profile);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Title == "Event 1" && e.Hash == "hash1");
        Assert.Contains(result, e => e.Title == "Event 2" && e.Hash == "hash2");
        Assert.All(result, e => Assert.True(e.StartTime < e.EndTime));
    }

    [Fact]
    public void SetEventRole_UpdatesRoleForProfileEvent()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventHash = "hash1";

        // Create Event
        var ev = new Event
        {
            Hash = eventHash,
            Title = "Test Event",
            StartTime = DateTimeOffset.Now,
            EndTime = DateTimeOffset.Now.AddHours(2)
        };

        // Create Profile
        var profile = new Profile
        {
            Id = 1,
        };

        // Create ProfileEvent with the required Profile and Event
        var profileEvent = new ProfileEvent
        {
            Profile = profile,   // Ensure Profile is set
            Event = ev,          // Ensure Event is set
            Role = EventRole.Owner
        };

        profile.ProfileEvents = new List<ProfileEvent> { profileEvent }; // Assign the ProfileEvent to the Profile's ProfileEvents

        dbContext.Events.Add(ev);
        dbContext.Profiles.Add(profile);
        dbContext.SaveChanges();

        var service = new ProfileService(dbContext);

        // Act
        service.SetEventRole(ev, profile, EventRole.Owner);

        // Assert
        var updatedProfileEvent = profile.ProfileEvents.First(pe => pe.Event.Hash == eventHash);
        Assert.Equal(EventRole.Owner, updatedProfileEvent.Role);
    }


    [Fact]
    public void SetEventRole_ThrowsException_IfEventNotFound()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var ev = new Event
        {
            Hash = "hash1",
            Title = "Test Event",
            StartTime = DateTimeOffset.Now,
            EndTime = DateTimeOffset.Now.AddHours(2)
        };

        var profile = new Profile
        {
            Id = 1,
            ProfileEvents = new List<ProfileEvent>() // No events added
        };

        dbContext.Events.Add(ev);
        dbContext.Profiles.Add(profile);
        dbContext.SaveChanges();

        var service = new ProfileService(dbContext);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => service.SetEventRole(ev, profile, EventRole.Owner));
        Assert.Equal("Event not found", exception.Message);
    }


}
