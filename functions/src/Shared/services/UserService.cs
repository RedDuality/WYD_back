using Model;
using Database;
using FirebaseAdmin.Auth;
using System.Linq.Expressions;

namespace Controller;
public class UserService
{

    WydDbContext db;

    private AccountService _accountService;
    private ProfileService _profileService;

    public UserService(WydDbContext wydDbContext, AccountService accountService, ProfileService profileService)
    {
        db = wydDbContext;
        
        _accountService = accountService;
        _profileService = profileService;
    }

    public User Get(int id)
    {

        return db.Users.Single(u => u.Id == id);

    }

    public User RetrieveFromAccountUid(String uid) {
        Account account = db.Accounts.Single(a => a.Uid == uid);
        if(account.User == null)
            throw new Exception("No User linked to this profile!");
        return account.User;
    }



    public User Create(UserRecord UR)
    {
        User user = new();
        user.MainMail = UR.Email;
        db.Users.Add(user);
        
        db.SaveChanges();

        Account account = new()
        {
            Mail = UR.Email,
            Uid = UR.Uid,
            User = user
        };
        this.AddAccount(user, account);

        Profile profile = new();
        this.AddProfile(user, profile);

        return user;
    }


    public User AddAccount(User user, Account account){
        _accountService.Create(account);
        user.Accounts.Add(account);
        db.SaveChanges();
        return user;
    }

    public User AddProfile(User user, Profile profile){
        _profileService.Create(profile);
        user.Profiles.Add(profile);
        db.SaveChanges();
        return user;
    }


    public User Update(User u, User newUser)
    {
        u.UserName = newUser.UserName;
        u.MainMail = newUser.MainMail;
        db.SaveChanges();
        return u;
    }

    //should this go in Profile service?
    public void SetRole(User user, Profile profile, Role role){
        var userRole = user.UserRoles.Find(ur => ur.Profile == profile);
        if (userRole == null)
            throw new Exception("Event not found");

        userRole.Role = role;

        db.SaveChanges();
    }

    //TODO make private
    public User Retrieve(User user)
    {
        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }


/*
    public string Delete(int id)
    {

        User user = db.Users.Include(u => u.Events).ThenInclude(e => e.UserEvents).Single(u => u.Id == id);
        List<Event> orphanEvents = user.Events.Where(e => e.UserEvents.Count == 1).ToList();
        db.Remove(user);
        db.Events.RemoveRange(orphanEvents);
        db.SaveChanges();
        return "Utente eliminato con successo";

    }
    */
}