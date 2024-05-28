using Model;
using Database;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;


namespace Controller;
public class EventController
{

    WydDbContext db;
    Mapper userMapper;

    public EventController(WydDbContext context)
    {
        db = context;
        var userMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateProjection<User, UserDto>();
            cfg.CreateProjection<Event, EventDto>();
            cfg.CreateProjection<UserEvent, UserEventDto>();
        });

        userMapper = new Mapper(userMapperConfig);
    }

    public bool MethodForTesting()
    {
        db = new WydDbContext();
        return db.Database.CanConnect();
    }

    
    public List<EventDto> GetEvents(int userId)
    {
        var user = userMapper.ProjectTo<UserDto>(db.Users, null).Single(u => u.Id == userId);
        //var user = db.Users.Include(u => u.Events).Single(u => u.Id == userId);
        return user.Events;

        throw new Exception("Error while fetching event data");

    }

    public Event Create(Event ev, User user)
    {
        var transaction = db.Database.BeginTransaction();
        ev.Id = null;
        db.Events.Add(ev);
        db.SaveChanges();
        ev.OwnerId = user.Id;
        ev.users.Add(user);
        ConfirmEvent(ev.Id, user);
        transaction.Commit();
        return ev;
    }

    public void ConfirmEvent(int? eventId, User user)
    {
        var events = user.Events;
        var userEvent = user.UserEvents.Find(ue => ue.EventId == eventId);
        if (userEvent == null)
            throw new Exception("Event not found");
        
        userEvent.confirmed = true;

        db.SaveChanges();
    }

    public void Decline(int eventId, User user)
    {

        var events = user.Events;
        var userEvent = user.UserEvents.Find(ue => ue.EventId == eventId);
        if (userEvent == null)
            throw new Exception("Event not found");
        
        userEvent.confirmed = false;

        db.SaveChanges();

    }

    //only for fields
    public string Update(Event ev)
    {
        //TODO check the user can modify it

        Event old = db.Events.Single(e => e.Id == ev.Id);
        old.update(ev);

        db.SaveChanges();
        return "Evento aggiornato con successo";

    }

    public Event Share(int eventId, List<int> usersId)
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

    public bool DeleteForUser(int eventId, int userId)
    {

        Event ev = db.Events.Include(e => e.users).Single(e => e.Id == eventId);
        User u = db.Users.Single(e => e.Id == userId);
        ev.users.Remove(u);
        if (ev.users.Count == 0)
            db.Remove(ev);
        db.SaveChanges();
        return true;

    }

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
        ev.ForEach(e => e.users.UnionWith(users));
        db.Events.UpdateRange(ev);
        db.SaveChanges();

        transaction.Commit();

        return ev;
    }
}