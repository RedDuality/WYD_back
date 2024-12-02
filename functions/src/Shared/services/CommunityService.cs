using Model;
using Database;
using Dto;

namespace Service;
public class CommunityService
{
    private readonly WydDbContext db;

    private readonly UserService userService;

    public CommunityService(WydDbContext context, UserService userService)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");

        this.userService = userService;
    }

    public Community? Retrieve(int id)
    {
        return db.Communities.Find(id);
    }


    public Community Create(CreateCommunityDto dto, User user)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Event cannot be null.");
        }

        using var transaction = db.Database.BeginTransaction();
        try
        {

            Community newCommunity = FromDto(dto, user);

            db.Communities.Add(newCommunity);
            db.SaveChanges();

            Group group = new()
            {
                Name = "General",
                GeneralForCommunity = true,
                Community = newCommunity,
                Users = newCommunity.Users,
            };

            AddNewGroup(newCommunity, group);

            transaction.Commit();

            return newCommunity;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Error creating community. Transaction rolled back.", ex);
        }
    }

    private Community FromDto(CreateCommunityDto dto, User user)
    {
        Community community = new();

        var users = GetUsers(dto.Users, user);

        if (dto.Type == CommunityType.Personal && users.Count != 2)
            throw new Exception("Users must be at least 2");

        if (users.Count <= 0) throw new Exception("Users list must not be empty");

        community.Type = dto.Type;
        community.Name = dto.Name ?? "My Community";
        community.Users = users;

        return community;
    }

    private HashSet<User> GetUsers(ICollection<UserDto> userDtos, User user)
    {
        HashSet<User> users = userService.GetUsers(userDtos);
        users.Add(user);
        return users;
    }

    private Community AddNewGroup(Community community, Group group)
    {
        community.Groups.Add(group);
        db.SaveChanges();

        return community;
    }

    public Community CreateAndAddNewGroup(Community community, GroupDto dto, User user)
    {
        //TODO check users are already in the commmunity
        Group group = new()
        {
            Name = dto.Name ?? "New Group",
            GeneralForCommunity = false,
            Community = community,
            Users = GetUsers(dto.Users, user),
        };

        return AddNewGroup(community, group);
    }

    public Community MakeMultiGroup(Community community)
    {
        if (community.Type == CommunityType.Personal)
            throw new Exception("Cannot transform this chat into a community");

        community.Type = CommunityType.Community;
        db.SaveChanges();
        return community;
    }

}