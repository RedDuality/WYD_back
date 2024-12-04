using Model;
using Database;
using Dto;

namespace Service;
public class CommunityService(WydDbContext context)
{
    private readonly WydDbContext db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");

    public Community? RetrieveOrNull(int id)
    {
        return db.Communities.Find(id);
    }

    public Community Retrieve(int id)
    {
        return RetrieveOrNull(id) ?? throw new KeyNotFoundException("Community");

    }
    public Community Create(CreateCommunityDto dto, Profile profile)
    {
        using var transaction = db.Database.BeginTransaction();
        try
        {
            Community newCommunity = FromDto(dto, profile);

            db.Communities.Add(newCommunity);
            db.SaveChanges();

            Group group = new()
            {
                Name = "General",
                GeneralForCommunity = true,
                Community = newCommunity,
                Profiles = newCommunity.Profiles,
            };

            AddGroup(newCommunity, group);

            transaction.Commit();

            return newCommunity;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Error creating community. Transaction rolled back.", ex);
        }
    }

    private static Community FromDto(CreateCommunityDto dto, Profile profile)
    {
        Community community = new();

        dto.Profiles.Add(profile);

        if (dto.Type == CommunityType.Personal && dto.Profiles.Count != 2)
            throw new Exception("Profiles must be at least 2");

        if (dto.Profiles.Count <= 0) throw new Exception("Profiles list must not be empty");

        community.Type = dto.Type;
        community.Name = dto.Name ?? "My Community";
        community.Profiles = dto.Profiles;

        return community;
    }

    public Community CreateAndAddGroup(Community community, GroupDto dto, Profile profile)
    {
        //TODO check profiles are already in the commmunity
        dto.Profiles.Add(profile);

        Group group = new()
        {
            Name = dto.Name ?? "New Group",
            GeneralForCommunity = false,
            Community = community,
            Profiles = dto.Profiles,
        };

        return AddGroup(community, group);
    }

    private Community AddGroup(Community community, Group group)
    {
        community.Groups.Add(group);
        db.SaveChanges();

        return community;
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