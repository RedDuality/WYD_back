using Model;
using Database;
using Dto;
using FirebaseAdmin.Auth;

namespace Service;
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

    public User? Retrieve(int id)
    {
        return db.Users.Find(id);
    }

    public async Task<User> GetOrCreateAsync(string uid)
    {
        Account? account = _accountService.Retrieve(uid);

        if (account == null) //registration
        {
            UserRecord UR = await AuthService.RetrieveFirebaseUser(uid);
            return Create(UR.Email, UR.Uid);
        }
        return account.User;
    }

    public HashSet<User> GetUsers(ICollection<UserDto> userDtos)
    {
        var userIds = userDtos.Select(u => u.Id).ToList();
        return GetUsers(userIds);
    }

    public HashSet<User> GetUsers(ICollection<int> userIds)
    {
        return db.Users.Where(u => userIds.Contains(u.Id)).ToHashSet();
    }

    private User Create(string Email, string Uid)
    {
        using var transaction = db.Database.BeginTransaction();
        try
        {
            // Create a new user
            var user = new User
            {
                MainMail = Email,
                UserName = Email,
                Tag = Email
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

    public List<UserDto> SearchByTag(string searchTag)
    {
        return db.Users
                 .Where(u => u.Tag.StartsWith(searchTag))
                 .Take(5)
                 .Select(u => new UserDto(u))
                 .ToList();
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