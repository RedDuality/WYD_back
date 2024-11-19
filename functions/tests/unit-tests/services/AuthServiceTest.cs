
using Controller;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Model;
using Moq;
namespace unit_tests.services;

public class AuthServiceTests
{

    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        return new WydDbContext(options);
    }

    private AuthService GetAuthService(WydDbContext dbContext)
    {
        var userService = new UserService(dbContext, new AccountService(dbContext), new ProfileService(dbContext));
        var accountService = new AccountService(dbContext);
        return new AuthService(userService, accountService);
    }

    [Fact]
    public void Test_GoogleCredentialsMissing_ThrowsException()
    {
        // Arrange: Simulate missing environment variable
        Environment.SetEnvironmentVariable("GOOGLE_CREDENTIALS", "");

        WydDbContext db = GetInMemoryDbContext();
        AccountService AS = new AccountService(db);
        // Act & Assert
        var exception = Assert.Throws<Exception>(() => new AuthService(new UserService(db, AS, new ProfileService(db)), AS));
        Assert.Equal("Google credentials not found in the environment variable", exception.Message);
    }

    //TODO test Google Credentials

}
