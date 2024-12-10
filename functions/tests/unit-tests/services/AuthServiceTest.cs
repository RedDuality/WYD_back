
using Service;
using Database;
using Microsoft.EntityFrameworkCore;
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
