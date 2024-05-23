using Model;
using Database;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;


namespace Controller;
public class EventController
{

    WydDbContext db;
    Mapper userMapper;

    public EventController()
    {
        db = new WydDbContext();
        var userMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateProjection<User, UserDto>();
            cfg.CreateProjection<Event, EventDto>();
            cfg.CreateProjection<UserEvent, UserEventDto>();
        });

        userMapper = new Mapper(userMapperConfig);
    }

    public bool MethodForTesting(){
        db = new WydDbContext();
        return db.Database.CanConnect();
    }
    public List<EventDto> GetEvents(int userId)
    {
        using (db)
        {
            var user = userMapper.ProjectTo<UserDto>(db.Users, null).Single(u => u.Id == userId);
            //var user = db.Users.Include(u => u.Events).Single(u => u.Id == userId);
            return user.Events;
        }
        throw new Exception("Error while fetching event data");

    }

    public Event Create(Event ev, User user)
    {
        var transaction = db.Database.BeginTransaction();
        User uc = new UserController().Get(2);
        ev.Id = null;
        db.Events.Add(ev);
        db.SaveChanges();
        ev.OwnerId = uc.Id;
        ev.users.Add(uc);
        db.SaveChanges();
        Confirm(ev, uc);
        transaction.Commit();
        return ev;

    }

    public void Confirm(Event ev, User user)
    {

        ev.UserEvents.First(ue => ue.UserId == user.Id).confirmed = true;
        db.SaveChanges();
    }

    public void Decline(Event ev, User user)
    {
        using (db)
        {
            ev.UserEvents.First(ue => ue.UserId == user.Id).confirmed = false;
            db.SaveChanges();
        }
    }

    //only for fields
    public string Update(Event ev)
    {
        //TODO check the user can modify it
        using (db)
        {
            Event old = db.Events.Single(e => e.Id == ev.Id);
            old.update(ev);

            db.SaveChanges();
            return "Evento aggiornato con successo";
        }
    }

    public Event Share(int eventId, List<int> usersId)
    {

        using (db)
        {
            //TODO check user has the event he wants to share
            Event ev;
            try
            {
                ev = db.Events.Include(e => e.users).Single(e => e.Id == eventId);
            }
            catch (InvalidOperationException)
            {
                throw new Exception("Evento non trovato");
            }
            var users = db.Users.Where(u => usersId.Contains(u.Id)).ToList();
            ev.users.UnionWith(users);
            db.SaveChanges();
            return ev;
        }

    }

    public bool DeleteForUser(int eventId, int userId)
    {
        using (db)
        {
            Event ev = db.Events.Include(e => e.users).Single(e => e.Id == eventId);
            User u = db.Users.Single(e => e.Id == userId);
            ev.users.Remove(u);
            if(ev.users.Count == 0)
                db.Remove(ev);
            db.SaveChanges();
            return true;
        }
    }

    public bool Delete(int id, int userId)
    {
        using (db)
        {
            Event ev = db.Events.Single(e => e.Id == id);
            if(ev.OwnerId != userId)
                return false;
            db.Remove(ev);
            db.SaveChanges();
            return true;
        }
    }

    public List<Event> AddMultiple(List<User> users, List<Event> ev)
    {
        using (db)
        {
            var transaction = db.Database.BeginTransaction();

            ev.ForEach(e => e.Id = null);

            db.Events.AddRange(ev);
            db.SaveChanges();
            ev.ForEach(e => e.users.UnionWith(users));
            db.Events.UpdateRange(ev);
            db.SaveChanges();

            transaction.Commit();

            return ev;
        }
    }
}