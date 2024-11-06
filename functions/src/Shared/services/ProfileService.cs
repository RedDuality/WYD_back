using Model;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace Controller;
public class ProfileService
{

    WydDbContext db;

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

    public void SetRole(Event ev, Profile profile, EventRole role){
        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id);
        if (profileEvent == null)
            throw new Exception("Event not found");

        profileEvent.eventRole = role;

        db.SaveChanges();
    }
}