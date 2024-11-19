using Database;


namespace unit_tests;

public class AccountTest
{
    readonly WydDbContext db;

    public AccountTest(){
        db = new WydDbContext();
    }

    [Fact]
    public void TestConnection()
    {
       Assert.True(db.Database.CanConnect());
    }
}