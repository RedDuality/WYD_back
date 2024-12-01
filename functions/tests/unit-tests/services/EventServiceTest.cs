

using Service;
using Database;
using Dto;
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
        var ev = new EventDto { Hash = "hash2", StartTime = DateTimeOffset.Now, EndTime = DateTimeOffset.Now.AddHours(2) };

        // Act
        var result = eventService.Create(ev, profile);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("hash2", result.Hash);
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

        Assert.Equal("Event cannot be null. (Parameter 'dto')", exception.Message);
    }

    [Fact]
    public void UpdateField_ValidDto_UpdatesFields()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        // Add an initial Event to the database
        var initialEvent = new Event
        {
            Id = 1,
            Title = "Old Title",
            Description = "Old Description",
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1)
        };
        dbContext.Events.Add(initialEvent);
        dbContext.SaveChanges();

        var dto = new EventDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(2)
        };

        var eventService = new EventService(dbContext);

        // Act
        eventService.UpdateField(dto);

        // Assert
        var updatedEvent = dbContext.Events.First(e => e.Id == 1);

        Assert.Equal("Updated Title", updatedEvent.Title);
        Assert.Equal("Updated Description", updatedEvent.Description);
        Assert.Equal(dto.StartTime, updatedEvent.StartTime);
        Assert.Equal(dto.EndTime, updatedEvent.EndTime);
    }

    [Fact]
    public void UpdateField_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var eventService = new EventService(dbContext);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => eventService.UpdateField(null));
        Assert.Equal("Event cannot be null. (Parameter 'dto')", exception.Message);
    }

    [Fact]
    public void UpdateField_NonExistentEvent_ThrowsInvalidOperationException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var dto = new EventDto
        {
            Id = -999, // Non-existent ID
            Title = "Updated Title",
            Description = "Updated Description",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(2)
        };

        var eventService = new EventService(dbContext);


        var exception = Assert.Throws<InvalidOperationException>(() => eventService.UpdateField(dto));

        // Assert
        var eventCount = dbContext.Events.Count();
        Assert.Equal(0, eventCount); // No events should exist
    }

    [Fact]
    public void UpdateField_DbSaveThrowsException_ThrowsInvalidOperationException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var dto = new EventDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            StartTime = DateTimeOffset.UtcNow.AddDays(1),
            EndTime = DateTimeOffset.UtcNow.AddDays(2)
        };

        var eventService = new EventService(dbContext);

        // Simulate a save exception by overriding SaveChanges behavior
        dbContext.Database.EnsureDeleted(); // Break the in-memory DB
        dbContext.Dispose();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => eventService.UpdateField(dto));
        Assert.Contains("Error updating event", exception.Message);
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
