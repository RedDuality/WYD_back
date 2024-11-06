


using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Model;

public enum Role {
    Owner,
    Admin,
    TicketChecker
}

[Table("User_Role")]
public class UserRole : BaseEntity
{

    
    [ForeignKey("ProfileId")]
    public required virtual Profile Profile {get; set;}
    [ForeignKey("UserId")]
    public required virtual User User {get; set;}
    
    [NotNull]
    public Role Role { get; set; } = Role.Owner;

    public DateTime? StartsAt {get; set;}
    public DateTime? EndsAt {get; set;}

}