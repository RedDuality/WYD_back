using Xunit.Sdk;
using Database;
using Controller;
using Model;

namespace unit_tests;

public class DbTest
{

    [Fact]
    public void TestConnection()
    {
       WydDbContext db = new WydDbContext();
       Environment.SetEnvironmentVariable("SqlConnectionString", "Server=tcp:wydreldbserver.database.windows.net,1433;Initial Catalog=Wydreldb;Persist Security Info=False;User ID=wydadmin;Password=password_1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

       Assert.True(db.Database.CanConnect());
    }
}