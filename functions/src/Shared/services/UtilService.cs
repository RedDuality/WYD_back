
using Database;

namespace Controller;
public class UtilService
{

    WydDbContext db;
    AuthService authService;

    public UtilService(WydDbContext context, AuthService authService)
    {
        db = context;

        this.authService = authService;
    }

    public bool PopulateDb()
    {

        var transaction = db.Database.BeginTransaction();



        transaction.Commit();
        return true;
    }



}