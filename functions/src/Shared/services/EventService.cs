using Model;
using Database;

namespace Controller;
public class EventService
{
    private readonly WydDbContext db;

    public EventService(WydDbContext context)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");
    }

    public Event Retrieve(int eventId)
    {
        try
        {
            // Retrieve event by Id
            return db.Events.Single(e => e.Id == eventId);
        }
        catch (InvalidOperationException ex)
        {
            // If no event is found or multiple events are found, throw a custom exception
            throw new KeyNotFoundException($"Event with ID {eventId} not found.", ex);
        }
    }

    public Event? RetrieveFromHash(string eventHash)
    {
        if (string.IsNullOrEmpty(eventHash))
        {
            throw new ArgumentException("Event hash cannot be null or empty.", nameof(eventHash));
        }

        return db.Events.FirstOrDefault(e => e.Hash == eventHash);
    }

    public Event Create(Event ev, Profile profile)
    {
        if (ev == null)
        {
            throw new ArgumentNullException(nameof(ev), "Event cannot be null.");
        }
        
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
        }

        using (var transaction = db.Database.BeginTransaction())
        {
            try
            {
                // Clear Id to force an insert (if necessary)
                ev.Id = 0;

                // Add the event to the database
                db.Events.Add(ev);
                db.SaveChanges();

                // Add the profile to the event
                ev.Profiles.Add(profile);
                db.SaveChanges();

                // Commit transaction if everything is successful
                transaction.Commit();

                return ev;
            }
            catch (Exception ex)
            {
                // If anything goes wrong, rollback the transaction
                transaction.Rollback();
                throw new InvalidOperationException("Error creating event. Transaction rolled back.", ex);
            }
        }
    }

    public void Confirm(Event ev, Profile profile)
    {
        if (ev == null)
        {
            throw new ArgumentNullException(nameof(ev), "Event cannot be null.");
        }
        
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
        }

        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
        {
            throw new KeyNotFoundException($"Event with ID {ev.Id} not found for the given profile.");
        }

        profileEvent.Confirmed = true;

        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error confirming event for profile.", ex);
        }
    }

    public void Decline(Event ev, Profile profile)
    {
        if (ev == null)
        {
            throw new ArgumentNullException(nameof(ev), "Event cannot be null.");
        }

        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");
        }

        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
        {
            throw new KeyNotFoundException($"Event with ID {ev.Id} not found for the given profile.");
        }

        profileEvent.Confirmed = false;

        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error declining event for profile.", ex);
        }
    }

    public Event Share(int eventId, List<Profile> profiles)
    {
        if (profiles == null || profiles.Count == 0)
        {
            throw new ArgumentException("Profiles list cannot be null or empty.", nameof(profiles));
        }

        // Retrieve event and check if it exists
        var ev = Retrieve(eventId);

        // Add the profiles to the event
        ev.Profiles.UnionWith(profiles);

        try
        {
            db.SaveChanges();
            return ev;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error sharing event with profiles.", ex);
        }
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