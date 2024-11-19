using Model;
using Database;

namespace Controller;
public class AccountService
{

    WydDbContext db;

    public AccountService(WydDbContext wydDbContext)
    {
        db = wydDbContext;
    }

    public Account Get(int id)
    {
        return db.Accounts.Single(a => a.Id == id);

    }
    public Account? Get(string uid)
    {
        return db.Accounts.SingleOrDefault(a => a.Uid == uid);

    }

    public Account Create(Account account)
    {
        db.Accounts.Add(account);
        db.SaveChanges();
        return account;
    }
}