using Model;
using Database;
using Dto;

namespace Controller;
public class CommunityService
{
    private readonly WydDbContext db;

    public CommunityService(WydDbContext context)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");
    }

    public CommunityDto Retrieve(int communityId, int userId)
    {
        try
        {
            // Retrieve community by Id
            return new CommunityDto(db.Communities.Single(c => c.Id == communityId), userId);
        }
        catch (InvalidOperationException ex)
        {

            throw new KeyNotFoundException($"Event with ID {communityId} not found.", ex);
        }
    }

    public Community Create(CreateCommunityDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Event cannot be null.");
        }

        using var transaction = db.Database.BeginTransaction();
        try
        {
            dto.Id = 0;
            Community newCommunity = Community.FromCreateDto(dto);

            var userIds = dto.Users.Select(u => u.Id).ToList();
            HashSet<User> users = db.Users.Where(u => userIds.Contains(u.Id)).ToHashSet();

            newCommunity.Users = users;

            db.Communities.Add(newCommunity);
            db.SaveChanges();


            Group group = new Group
            {
                Name = dto.Name ?? "General",
                GeneralForCommunity = true,
                Community = newCommunity,
                Users = users,
            };

            db.Groups.Add(group);

            db.SaveChanges();

            transaction.Commit();

            return newCommunity;
        }
        catch (Exception ex)
        {
            // If anything goes wrong, rollback the transaction
            transaction.Rollback();
            throw new InvalidOperationException("Error creating community. Transaction rolled back.", ex);
        }
    }

}