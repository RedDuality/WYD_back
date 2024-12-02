using Service;
using Database;
using Microsoft.EntityFrameworkCore;
using Model;


namespace unit_tests.services;

public class AccountServiceTest
{

    private static WydDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<WydDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        return new WydDbContext(options);
    }

    [Fact]
    public void Get_ById_ReturnsAccount()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var newAccount = dbContext.Accounts.Add(new Account { Uid = "123", Mail = "mail1@mail.com" });
        dbContext.SaveChanges();

        var service = new AccountService(dbContext);

        // Act
        var result = service.Retrieve(newAccount.Entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void Get_ByUid_ReturnsAccount()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Accounts.Add(new Account { Uid = "1234", Mail = "mail2@mail.com" });
        dbContext.SaveChanges();

        var service = new AccountService(dbContext);

        // Act
        var result = service.Retrieve("1234");

        // Asser
        Assert.NotNull(result);
        Assert.Equal("1234", result!.Uid);
    }

    [Fact]
    public void Create_AddsAccountToDbContext()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new AccountService(dbContext);
        var newAccount = new Account { Uid = "456", Mail = "mail@mail.com" };

        // Act
        var result = service.Create(newAccount);

        // Assert
        var createdAccount = dbContext.Accounts.Find(result.Id);
        Assert.NotNull(createdAccount);
        Assert.Equal("456", createdAccount!.Uid);
    }

    [Fact]
    public void Create_ValidAccount_ReturnsAccount()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new AccountService(dbContext);
        var account = new Account
        {
            Mail = "test@example.com",
            Uid = "uid123"
        };

        // Act
        var result = service.Create(account);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Mail);
        Assert.Equal("uid123", result.Uid);
    }

    [Fact]
    public void Create_AccountWithEmptyMail_ThrowsArgumentException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new AccountService(dbContext);
        var account = new Account
        {
            Mail = string.Empty,
            Uid = "uid123"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.Create(account));
        Assert.Equal("Invalid account data provided.", exception.Message);
    }

    [Fact]
    public void Create_AccountWithEmptyUid_ThrowsArgumentException()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new AccountService(dbContext);
        var account = new Account
        {
            Mail = "test@example.com",
            Uid = string.Empty
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => service.Create(account));
        Assert.Equal("Invalid account data provided.", exception.Message);
    }

}