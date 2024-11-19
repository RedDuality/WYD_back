using Controller;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Model;
using Moq;

namespace unit_tests.services;

public class UserServiceTest
{
    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new WydDbContext(options);
    }

    [Fact]
    public void Get_ValidUserId_ReturnsUser()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userService = new UserService(dbContext, new AccountService(dbContext), new ProfileService(dbContext));
        var user = new User { UserName = "TestUser" };
        dbContext.Users.Add(user);
        dbContext.SaveChanges();

        // Act
        var result = userService.Get(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public void Get_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userService = new UserService(dbContext, new AccountService(dbContext), new ProfileService(dbContext));

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => userService.Get(999)); // Non-existent user
        Assert.Contains("User with ID 999 not found", exception.Message);
    }

    [Fact]
    public void Create_ValidEmailAndUid_CreatesUser()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userService = new UserService(dbContext, new AccountService(dbContext), new ProfileService(dbContext));

        string email = "test1@mail.com";
        string uid = "uid1234";

        // Act
        var user = userService.Create(email, uid);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.MainMail);
        Assert.NotNull(user.MainProfile); // Ensure profile is created
        Assert.NotNull(user.Accounts); // Ensure account is created
        Assert.Single(user.Accounts); // Ensure only one account is created
    }

    [Fact]
    public void SetProfileRole_ProfileNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userService = new UserService(dbContext, new AccountService(dbContext), new ProfileService(dbContext));
        var user = new User { UserName = "TestUser" };
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        var profile = new Profile();

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() => userService.SetProfileRole(user, profile, Role.Owner));
        Assert.Contains("User does not have the specified profile.", exception.Message);
    }
}
