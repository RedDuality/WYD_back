

using Controller;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Model;

namespace unit_tests.services;

public class EventServiceTest
{
    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Unique DB for each test
            .Options;

        return new WydDbContext(options);
    }

    [Fact]
    public void Retrieve_ValidEvent_ReturnsEvent()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);
        var event1 = new Event { Hash = "hash1", StartTime = DateTimeOffset.Now, EndTime = DateTimeOffset.Now.AddHours(2) };
        dbContext.Events.Add(event1);
        dbContext.SaveChanges();

        // Act
        var result = eventService.Retrieve(event1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(event1.Id, result.Id);
    }

    [Fact]
    public void Retrieve_EventNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => eventService.Retrieve(999)); // Non-existent event
        Assert.Contains("Event with ID 999 not found", exception.Message);
    }

    [Fact]
    public void Create_ValidEventAndProfile_CreatesEvent()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);
        var profile = new Profile { };
        var ev = new Event { Hash = "hash2", StartTime = DateTimeOffset.Now, EndTime = DateTimeOffset.Now.AddHours(2) };

        // Act
        var result = eventService.Create(ev, profile);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("hash2", result.Hash);
    }

    [Fact]
    public void Create_EventNull_ThrowsArgumentNullException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);

        // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var exception = Assert.Throws<ArgumentNullException>(() => eventService.Create(null, new Profile()));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        Assert.Equal("Event cannot be null. (Parameter 'ev')", exception.Message);
    }

    [Fact]
    public void Confirm_EventNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);
        var profile = new Profile { };
        var event1 = new Event { Hash = "hash3", StartTime = DateTimeOffset.Now, EndTime = DateTimeOffset.Now.AddHours(2) };
        dbContext.Events.Add(event1);
        dbContext.SaveChanges();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => eventService.Confirm(event1, new Profile())); // Profile doesn't have event
        Assert.Contains("Event with ID", exception.Message);
    }
}
