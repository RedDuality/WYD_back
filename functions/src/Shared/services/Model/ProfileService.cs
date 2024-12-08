using Model;
using Database;
using Dto;
using Microsoft.AspNetCore.Http;

namespace Service;
public class ProfileService(WydDbContext wydDbContext)
{

    readonly WydDbContext db = wydDbContext;

    public Profile? RetrieveOrNull(int id)
    {
        return db.Profiles.Find(id);

    }

    public Profile Retrieve(int id)
    {
        return db.Profiles.Find(id) ?? throw new KeyNotFoundException("Profile");

    }

    public Profile RetrieveByHash(string hash)
    {
        return db.Profiles.Where(p => p.Hash.Equals(hash)).First() ?? throw new KeyNotFoundException("Profile");
    }

    public Profile RetrieveFromHeaders(HttpRequest req)
    {

        if (req.Headers.TryGetValue("Current-Profile", out var profileHash))
            return RetrieveByHash(profileHash!);
        else
            throw new ArgumentException("Profile header not found or in the wrong format");

    }

    public Profile Create(Profile profile)
    {
        db.Profiles.Add(profile);
        db.SaveChanges();
        return profile;
    }

    public static List<EventDto> RetrieveEvents(Profile profile)
    {
        return profile.Events.Select(ev => new EventDto(ev)).ToList();
    }

    public void SetEventRole(Event ev, Profile profile, EventRole role)
    {
        var profileEvent = profile.ProfileEvents.Find(pe => pe.Event.Id == ev.Id) ?? throw new KeyNotFoundException("ProfileEvent");

        profileEvent.Role = role;

        db.SaveChanges();
    }

    public List<Profile> SearchByTag(string searchTag)
    {
        return db.Profiles
                 .Where(u => u.Tag.StartsWith(searchTag))
                 .Take(5).ToList();
    }

}