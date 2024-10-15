using Database;


namespace unit_tests;

public class DbTest
{

    public DbTest(){
        Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wyddbserver.database.windows.net,1433;Initial Catalog=WYD-p-db;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }

    [Fact]
    public void TestConnection()
    {
        WydDbContext db = new WydDbContext();
       
       Assert.True(db.Database.CanConnect());
    }
}