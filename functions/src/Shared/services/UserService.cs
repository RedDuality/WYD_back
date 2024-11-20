using Model;
using Database;
using Dto;

namespace Controller;
public class UserService
{
    private readonly WydDbContext db;
    private readonly AccountService _accountService;
    private readonly ProfileService _profileService;

    public UserService(WydDbContext context, AccountService accountService, ProfileService profileService)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService), "Account service cannot be null.");
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService), "Profile service cannot be null.");
    }

    public User Get(int id)
    {
        try
        {
            return db.Users.Single(u => u.Id == id);
        }
        catch (InvalidOperationException ex)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.", ex);
        }
    }

    public User Get(string uid)
    {
        try
        {
            var account = db.Accounts.Single(a => a.Uid == uid);
            if (account.User == null)
                throw new InvalidOperationException("No User linked to this account.");

            return account.User;
        }
        catch (InvalidOperationException ex)
        {
            throw new KeyNotFoundException($"Account with UID {uid} not found or user is not linked.", ex);
        }
    }

    public User Create(string Email, string Uid)
    {
        var transaction = db.Database.BeginTransaction();
        try
        {
            // Create a new user
            var user = new User
            {
                MainMail = Email
            };
            db.Users.Add(user);
            db.SaveChanges();

            // Create associated account
            var account = new Account
            {
                Mail = Email,
                Uid = Uid,
                User = user
            };

            _accountService.Create(account);

            // Create associated profile
            var profile = new Profile();
            user.MainProfile = profile;
            AddProfile(user, profile);

            transaction.Commit();
            return user;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Error creating user and related entities. Transaction rolled back.", ex);
        }
    }

    public User AddProfile(User user, Profile profile)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");
        if (profile == null) throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");

        try
        {
            _profileService.Create(profile);
            user.Profiles.Add(profile);
            db.SaveChanges();
            return user;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error adding profile to user.", ex);
        }
    }

    public User Update(User existingUser, User updatedUser)
    {
        if (existingUser == null) throw new ArgumentNullException(nameof(existingUser), "Existing user cannot be null.");
        if (updatedUser == null) throw new ArgumentNullException(nameof(updatedUser), "Updated user cannot be null.");

        try
        {
            existingUser.UserName = updatedUser.UserName;
            existingUser.MainMail = updatedUser.MainMail;
            db.SaveChanges();
            return existingUser;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating user.", ex);
        }
    }

    public async Task<List<EventDto>> RetrieveEventsAsync(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

        var tasks = user.Profiles.Select(async profile =>
        {
            try
            {
                return await Task.FromResult(ProfileService.RetrieveEvents(profile));
            }
             catch (Exception ex)
            {
                // In case of an error, return a single EventDto with an error message
                return [
                new( new Event {
                    Id = -1,
                    Title = $"Error retrieving events for profile {profile.Id}",
                    Description = ex.Message,
                    StartTime = DateTime.MinValue,
                    EndTime = DateTime.MinValue,
                })];
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.SelectMany(result => result).ToList();
    }

    public void SetProfileRole(User user, Profile profile, Role role)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");
        if (profile == null) throw new ArgumentNullException(nameof(profile), "Profile cannot be null.");

        var userRole = user.UserRoles.Find(ur => ur.Profile == profile);
        if (userRole == null)
        {
            throw new KeyNotFoundException("User does not have the specified profile.");
        }

        userRole.Role = role;

        try
        {
            db.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error updating user profile role.", ex);
        }
    }

    /*
        public string Delete(int id)
        {

            User user = db.Users.Include(u => u.Events).ThenInclude(e => e.UserEvents).Single(u => u.Id == id);
            List<Event> orphanEvents = user.Events.Where(e => e.UserEvents.Count == 1).ToList();
            db.Remove(user);
            db.Events.RemoveRange(orphanEvents);
            db.SaveChanges();
            return "Utente eliminato con successo";

        }
        */
}