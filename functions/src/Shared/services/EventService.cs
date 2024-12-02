using Model;
using Database;
using Dto;

namespace Service;
public class EventService
{
    private readonly WydDbContext db;

    public EventService(WydDbContext context)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");
    }

    public Event? Retrieve(int eventId)
    {
        return db.Events.Find(eventId);
    }

    public Event? RetrieveFromHash(string eventHash)
    {
        if (string.IsNullOrEmpty(eventHash))
        {
            throw new ArgumentException("Event hash cannot be null or empty.", nameof(eventHash));
        }

        return db.Events.FirstOrDefault(e => e.Hash == eventHash);
    }

    public EventDto Create(EventDto dto, Profile profile)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Event cannot be null.");
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
                dto.Id = 0;
                Event newEvent = Event.FromDto(dto);
                // Add the event to the database
                db.Events.Add(newEvent);
                db.SaveChanges();


                // Add the profile to the event
                newEvent.Profiles.Add(profile);

                db.SaveChanges();
                if (dto.ProfileEvents.Count != 0)
                {
                    bool confirmed = dto.ProfileEvents.First().Confirmed;
                    if (!confirmed)
                    {
                        newEvent.ProfileEvents.First().Confirmed = false;
                        db.SaveChanges();
                    }
                }

                // Commit transaction if everything is successful
                transaction.Commit();

                return new EventDto(newEvent);
            }
            catch (Exception ex)
            {
                // If anything goes wrong, rollback the transaction
                transaction.Rollback();
                throw new InvalidOperationException("Error creating event. Transaction rolled back.", ex);
            }
        }
    }

    public Event UpdateField(EventDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Event cannot be null.");
        }

        try
        {
            Event eventToUpdate = Retrieve(dto.Id) ?? throw new KeyNotFoundException($"Event with ID {dto.Id} not found.");
            eventToUpdate.Update(dto);
            db.SaveChanges();
            return eventToUpdate;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating event", ex);
        }

    }

    public Event Share(int eventId, List<Profile> profiles)
    {
        if (profiles == null || profiles.Count == 0)
        {
            throw new ArgumentException("Profiles list cannot be null or empty.", nameof(profiles));
        }

        Event ev = Retrieve(eventId) ?? throw new KeyNotFoundException($"Event with ID {eventId} not found.");

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

    public void Confirm(Event ev, Profile? profile)
    {
        ChangeConfirmStatus(ev, profile, true);
    }

    public void Decline(Event ev, Profile? profile)
    {
        ChangeConfirmStatus(ev, profile, false);
    }

    private void ChangeConfirmStatus(Event ev, Profile? profile, bool confirmed)
    {
        if (ev == null) throw new ArgumentNullException(nameof(ev), "Event cannot be null.");


        if (profile == null) throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");


        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id) ?? throw new KeyNotFoundException($"ProfileEvent with ID {ev.Id} not found for the given profile.");

        profileEvent.Confirmed = confirmed;

        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error confirming or declining event for profile.", ex);
        }
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