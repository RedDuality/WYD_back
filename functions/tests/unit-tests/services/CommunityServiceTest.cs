using Service;
using Database;
using Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Model;


namespace unit_tests.services;

public class CommunityServiceTest
{

    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new WydDbContext(options);
    }

    [Fact]
    public void Create_ShouldCreateCommunity_WhenDtoIsValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CommunityService(context);

        var dto = new CreateCommunityDto
        {
            Name = "Test Community",
            Users = new HashSet<UserDto>
                {
                    new UserDto { Id = 1 },
                    new UserDto { Id = 2 }
                }
        };

        context.Users.AddRange(new User { Id = 1 }, new User { Id = 2 });
        context.SaveChanges();

        // Act
        var result = service.Create(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Community", result.Name);
        Assert.Equal(2, result.Users.Count);
        Assert.True(context.Communities.Any(c => c.Name == "Test Community"));
        Assert.True(context.Groups.Any(g => g.Name == "Test Community" && g.Community.Id == result.Id));
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenDtoIsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CommunityService(context);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.Create(null));
    }

    [Fact]
    public void Create_ShouldRollbackTransaction_WhenExceptionIsThrown()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new CommunityService(context);

        var dto = new CreateCommunityDto
        {
            Name = "Test Community",
            Users = new HashSet<UserDto>
                {
                    new UserDto { Id = 1 },
                    new UserDto { Id = 2 }
                }
        };

        // Simulate an exception by not adding users to the context
        // context.Users.AddRange(new User { Id = 1 }, new User { Id = 2 });
        // context.SaveChanges();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.Create(dto));
        Assert.False(context.Communities.Any(c => c.Name == "Test Community"));
    }
}
