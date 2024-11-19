using Model;
using Database;

namespace Controller;
public class EventService
{

    WydDbContext db;

    public EventService(WydDbContext context)
    {
        db = context;
    }

    public Event Retrieve(int eventId)
    {
        Event ev;
        try
        {
            ev = db.Events.Single(e => e.Id == eventId);
        }
        catch (InvalidOperationException)
        {
            throw new Exception("Evento non trovato");
        }
        return ev;
    }

    public Event? RetrieveFromHash(string eventHash)
    {
        return db.Events.FirstOrDefault(e => e.Hash == eventHash);
    }


    public Event Create(Event ev, Profile profile)
    {
        var transaction = db.Database.BeginTransaction();
        ev.Id = 0;
        db.Events.Add(ev);
        db.SaveChanges();
        ev.Profiles.Add(profile);
        db.SaveChanges();
        transaction.Commit();
        return ev;
    }

    
    public void Confirm(Event ev, Profile profile)
    {
        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
            throw new Exception("Event not found");

        profileEvent.Confirmed = true;

        db.SaveChanges();
    }

    public void Decline(Event ev, Profile profile)
    {
        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
            throw new Exception("Event not found");

        profileEvent.Confirmed = false;

        db.SaveChanges();
    }



    public Event Share(int eventId, List<Profile> profiles)
    {
        //TODO check user has the event he wants to share
        Event ev = Retrieve(eventId);
        ev.Profiles.UnionWith(profiles);
        //TODO check groups 
        db.SaveChanges();
        return ev;

    }



    public Event ConfirmFromHash(User user, string eventHash, bool confirm)
    {

        Event ev;
        try
        {
            ev = db.Events.Single(e => e.Hash == eventHash);
        }
        catch (InvalidOperationException)
        {
            throw new Exception("Evento non trovato");
        }
        var transaction = db.Database.BeginTransaction();
        //ev.Users.Add(user);
        db.SaveChanges();
        if (confirm)
            //ConfirmEvent(ev, user);

        transaction.Commit();

        return ev;

    }

/*
    public bool DeleteForUser(int eventId, int userId)
    {

        Event ev = db.Events.Include(e => e.Users).Single(e => e.Id == eventId);
        User u = db.Users.Single(e => e.Id == userId);
        ev.Users.Remove(u);
        if (ev.Users.Count == 0)
            db.Remove(ev);
        db.SaveChanges();
        return true;

    }
*/
/*
    public bool Delete(int id, int userId)
    {
        Event ev = db.Events.Single(e => e.Id == id);
        if (ev.OwnerId != userId)
            return false;
        db.Remove(ev);
        db.SaveChanges();
        return true;

    }

    public List<Event> AddMultiple(List<User> users, List<Event> ev)
    {

        var transaction = db.Database.BeginTransaction();

        ev.ForEach(e => e.Id = null);

        db.Events.AddRange(ev);
        db.SaveChanges();
        ev.ForEach(e => e.Users.UnionWith(users));
        db.Events.UpdateRange(ev);
        db.SaveChanges();

        transaction.Commit();

        return ev;
    }*/
}