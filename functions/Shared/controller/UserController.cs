using Model;
using Database;
using Azure.Core.GeoJson;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;


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

    public User Create(User user)
    {
        using (db)
        {
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
        using(db){
            User user = db.Users.Include(u => u.Events).ThenInclude(e => e.UserEvents).Single(u => u.Id == id);
            List<Event> orphanEvents = user.Events.Where(e => e.UserEvents.Count == 1).ToList();
            db.Remove(user);
            db.Events.RemoveRange(orphanEvents);
            db.SaveChanges();
            return "Utente eliminato con successo";
        }
       
    }
}