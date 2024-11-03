using Model;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

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
        return db.Accounts.Single(c => c.Id == id);

    }

    public Account Create(Account account)
    {
        db.Accounts.Add(account);
        db.SaveChanges();
        return account;
    }
}