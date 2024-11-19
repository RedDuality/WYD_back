using Database;


namespace unit_tests;

public class DbTest
{
    readonly WydDbContext db;

    public DbTest(){
       db = DbContextSingleton.Instance;
    }

    [Fact]
    public void TestConnection()
    {
       Assert.True(db.Database.CanConnect());
    }
}