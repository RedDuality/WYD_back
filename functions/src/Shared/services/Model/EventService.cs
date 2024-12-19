using Model;
using Database;
using Dto;

namespace Service;
public class EventService(WydDbContext context, GroupService groupService)
{
    private readonly WydDbContext db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");

    private readonly GroupService groupService = groupService ?? throw new ArgumentNullException(nameof(context), "GroupService cannot be null");

    public Event? RetrieveOrNull(int eventId)
    {
        return db.Events.Find(eventId);
    }
    public Event Retrieve(int eventId)
    {
        return db.Events.Find(eventId) ?? throw new KeyNotFoundException($"Event with ID {eventId} not found.");
    }

    public Event RetrieveByHash(string eventHash){
        return db.Events.FirstOrDefault(e => e.Hash == eventHash) ?? throw new KeyNotFoundException($"Event with ID {eventHash} not found.");
    }

    public Event SharedWithHash(string eventHash, Profile profile)
    {
        if (string.IsNullOrEmpty(eventHash))
        {
            throw new ArgumentException("Event hash cannot be null or empty.", nameof(eventHash));
        }

        Event ev = RetrieveByHash(eventHash);
        
        ev.Profiles.Add(profile);
        db.SaveChanges();

        return ev;
    }

    private Event CreateNewAndSave(EventDto eventDto)
    {
        Event newEvent = Event.FromDto(eventDto);

        db.Events.Add(newEvent);
        db.SaveChanges();
        return newEvent;
    }

    public async Task<Event> Create(EventDto dto, Profile profile)
    {
        Event newEvent;
        using var transaction = db.Database.BeginTransaction();
        try
        {
            newEvent = CreateNewAndSave(dto);
            Share(newEvent, [profile]);//add profile to event

            if (dto.ProfileEvents.Count != 0 && dto.ProfileEvents.First().Confirmed)
                ConfirmOrDecline(newEvent, profile, true);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Error creating event. Transaction rolled back.", ex);
        }

        await AddMultipleBlobs(newEvent, dto.NewBlobData);

        return newEvent;
    }

    public async Task<Event> UpdateAsync(EventDto dto)
    {
        Event eventToUpdate;
        try
        {
            eventToUpdate = RetrieveByHash(dto.Hash!);
            eventToUpdate.Update(dto);
            db.SaveChanges();

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating event", ex);
        }

        //TODO check for removed images

        await AddMultipleBlobs(eventToUpdate, dto.NewBlobData);

        return eventToUpdate;

    }

    private async Task AddMultipleBlobs(Event ev, HashSet<BlobData> blobDatas)
    {
        if (blobDatas.Count > 0)
        {
            List<Task> tasks = blobDatas.Select(async bd => await AddBlobAsync(ev, bd)).ToList();
            await Task.WhenAll(tasks);
        }
    }

    public async Task AddBlobAsync(string eventHash, BlobData blobData)
    {
        Event ev = RetrieveByHash(eventHash);
        await AddBlobAsync(ev, blobData);
    }

    private async Task AddBlobAsync(Event ev, BlobData blobData)
    {
        Blob newBlob = new();
        db.Blobs.Add(newBlob);
        ev.Blobs.Add(newBlob);

        await BlobService.UploadBlobAsync(ev.Hash, newBlob, blobData);
        db.SaveChanges();
    }

    internal Event ShareToGroups(string eventHash, HashSet<int> groupIds)
    {
        Event ev = RetrieveByHash(eventHash);

        var groups = groupService.Retrieve(groupIds).ToList();
        var profiles = groups.SelectMany(g => g.Profiles).ToHashSet();

        if (profiles.Count == 0) throw new Exception("No Profile to add this event to!");

        return Share(ev, profiles!);
    }

    public Event Share(Event ev, HashSet<Profile> profiles)
    {
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

    public void ConfirmOrDecline(string eventHash, Profile profile, bool confirmed)
    {
        Event ev = RetrieveByHash(eventHash);
        ConfirmOrDecline(ev, profile, confirmed);
    }

    public void ConfirmOrDecline(Event ev, Profile profile, bool confirmed)
    {
        var profileEvent = ev.ProfileEvents.Find(pe => pe.Profile.Id == profile.Id) ?? throw new KeyNotFoundException($"ProfileEvent with Profile ID {profile.Id} not found for the given profile.");
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