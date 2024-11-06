using Model;
using Database;

namespace Controller;
public class UserService
{

    WydDbContext db;

    public UserService(WydDbContext wydDbContext)
    {
        db = wydDbContext;
    }

    public User Get(int id)
    {

        return db.Users.Single(u => u.Id == id);

    }



    //TODO make private
    public User Create(User user)
    {
        db.Users.Add(user);
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