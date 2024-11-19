using Model;
using Database;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            if (string.IsNullOrEmpty(account.Mail) || string.IsNullOrEmpty(account.Uid))
            {
                throw new ArgumentException("Account Mail and Uid cannot be empty.");
            }

            db.Accounts.Add(account);
            db.SaveChanges();
            return account;
        }
        catch (DbUpdateException dbEx)
        {
            // Handle database-related issues, e.g., constraint violations
            throw new InvalidOperationException("Error saving account to the database.", dbEx);
        }
        catch (ArgumentException argEx)
        {
            // Handle invalid input (e.g., empty Mail or Uid)
            throw new ArgumentException("Invalid account data provided.", argEx);
        }
        catch (Exception ex)
        {
            // Handle any unexpected exceptions
            throw new Exception("An unexpected error occurred while creating the account.", ex);
        }
    }

}