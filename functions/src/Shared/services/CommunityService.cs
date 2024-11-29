using Model;
using Database;
using Dto;
using Microsoft.EntityFrameworkCore;

namespace Controller;
public class CommunityService
{
    private readonly WydDbContext db;

    public CommunityService(WydDbContext context)
    {
        db = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null");
    }

    public CommunityDto Retrieve(int communityId)
    {
        try
        {
            // Retrieve community by Id
            return new CommunityDto(db.Communities.Single(c => c.Id == communityId));
        }
        catch (InvalidOperationException ex)
        {

            throw new KeyNotFoundException($"Event with ID {communityId} not found.", ex);
        }
    }

    public CommunityDto Create(CreateCommunityDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Event cannot be null.");
        }

        using var transaction = db.Database.BeginTransaction();
        try
        {
            Community newCommunity = Community.FromCreateDto(dto);

            var userIds = dto.Users.Select(u => u.Id).ToList();
            HashSet<User> users = db.Users.Where(u => userIds.Contains(u.Id)).ToHashSet();

            newCommunity.Users = users;

            db.Communities.Add(newCommunity);
            db.SaveChanges();


            Group group = new Group{
                Name = dto.Name ?? "General",
                GeneralForCommunity = true,
                Community = newCommunity,
                Users = users,
            };

            db.Groups.Add(group);

            db.SaveChanges();

            transaction.Commit();

            return new CommunityDto(newCommunity);
        }
        catch (Exception ex)
        {
            // If anything goes wrong, rollback the transaction
            transaction.Rollback();
            throw new InvalidOperationException("Error creating community. Transaction rolled back.", ex);
        }
    }

}