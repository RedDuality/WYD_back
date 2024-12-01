using Model;
using Database;
using Dto;

namespace Service;
public class ProfileService
{

    readonly WydDbContext db;

    public ProfileService(WydDbContext wydDbContext)
    {
        db = wydDbContext;
    }

    public Profile Get(int id)
    {
        return db.Profiles.Single(p => p.Id == id);

    }

    public Profile Create(Profile profile)
    {
        db.Profiles.Add(profile);
        db.SaveChanges();
        return profile;
    }

    public List<Profile> GetProfiles(List<int> profileIds){
        return db.Profiles.Where(p => profileIds.Contains(p.Id)).ToList();
    }

    public static List<EventDto> RetrieveEvents(Profile profile){
        return profile.Events.Select(ev => new EventDto(ev)).ToList();
    }

    public void SetEventRole(Event ev, Profile profile, EventRole role){
        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
            throw new Exception("Event not found");

        profileEvent.Role = role;

        db.SaveChanges();
    }

}