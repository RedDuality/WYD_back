using Model;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Controller;
public class UserController
{

    WydDbContext db;

    public UserController()
    {
        db = new WydDbContext();
    }

    public User Get(int id)
    {
        using (db)
        {
            return db.Users.Single(u => u.Id == id);
        }

    }

    public User RetrieveByMail(string mail)
    {
        using (db)
        {
            return db.Users.Single(u => u.mail.Equals(mail));
        }

    }

    //TODO make private
    public User Create(User user)
    {
        using (db)
        {   
            //TODO add control over unique mail
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }
    }

    public User Update(int id, User newUser)
    {
        User u = Get(id);
        u.username = newUser.username;
        u.mail = newUser.mail;
        db.SaveChanges();
        return u;
    }

    public string Delete(int id)
    {
        using (db)
        {
            User user = db.Users.Include(u => u.Events).ThenInclude(e => e.UserEvents).Single(u => u.Id == id);
            List<Event> orphanEvents = user.Events.Where(e => e.UserEvents.Count == 1).ToList();
            db.Remove(user);
            db.Events.RemoveRange(orphanEvents);
            db.SaveChanges();
            return "Utente eliminato con successo";
        }

    }
}