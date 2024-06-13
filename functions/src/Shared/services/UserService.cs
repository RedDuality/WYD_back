using Model;
using Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

    public User RetrieveByMail(string mail)
    {

        return db.Users.Single(u => u.Mail.Equals(mail));

    }

    //TODO make private
    public User Create(User user)
    {
        //TODO add control over unique mail
        db.Users.Add(user);
        db.SaveChanges();
        return user;

    }

    public User Update(User u, User newUser)
    {
        u.Username = newUser.Username;
        u.Mail = newUser.Mail;
        db.SaveChanges();
        return u;
    }

    public string Delete(int id)
    {

        User user = db.Users.Include(u => u.Events).ThenInclude(e => e.UserEvents).Single(u => u.Id == id);
        List<Event> orphanEvents = user.Events.Where(e => e.UserEvents.Count == 1).ToList();
        db.Remove(user);
        db.Events.RemoveRange(orphanEvents);
        db.SaveChanges();
        return "Utente eliminato con successo";

    }
}