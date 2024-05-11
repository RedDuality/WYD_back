using Model;
using Database;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;


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

        using (db)
        {
            var transaction = db.Database.BeginTransaction();
            User uc = new UserController().Get(1);
            ev.Id = null;
            db.Events.Add(ev);
            db.SaveChanges();
            ev.users.Add(uc);
            db.Events.Update(ev);
            db.SaveChanges();
            transaction.Commit();
            return ev;
        }
    }

    //only for fields
    public string Update(Event ev)
    {

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

            db.SaveChanges();
            return true;
        }
    }

    public bool Delete(int id)
    {
        using (db)
        {//TODO check owner
            db.Remove(db.Events.Single(e => e.Id == id));
            db.SaveChanges();
            return true;
        }
    }

    public List<Event> Add(List<User> users, List<Event> ev)
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