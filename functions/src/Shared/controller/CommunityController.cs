using Model;
using Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;

namespace Controller;
public class CommunityController
{

    WydDbContext db;

    public CommunityController(WydDbContext wydDbContext)
    {
        db = wydDbContext;
    }

    public Community Get(int id)
    {

        return db.Communities.Single(c => c.Id == id);

    }

    //TODO make private
    public string Create(User user, string name, List<int> userIdList)
    {
        //TODO add control over unique mail
        
        var users = db.Users.Where(u => userIdList.Contains(u.Id));
        db.Communities.Add(new Community {Name = name, Users = users.ToList() });
        db.SaveChanges();

        return "gruppo creato con successo";
    }

    public Community Update(User user, int communityId, Community newCommunity)
    {
        //TODO check the user can update the community
        var toUpdate = db.Communities.First(c => c.Id == communityId);
        toUpdate.Name = newCommunity.Name;
        toUpdate.Users = newCommunity.Users;
        db.SaveChanges();
        return toUpdate;
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