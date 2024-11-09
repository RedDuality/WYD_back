


using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Model;

public enum Role {
    Owner,
    Admin,
    Viewer,
    TicketChecker
}

[Table("User_Role")]
public class UserRole : BaseEntity
{

    
    [ForeignKey("ProfileId")]
    public required virtual Profile Profile {get; set;}
    [JsonIgnore]
    [ForeignKey("UserId")]
    public required virtual User User { get; set; }
    
    [NotNull]
    public Role Role { get; set; } = Role.Owner;

    [JsonIgnore]
    public DateTime? StartsAt {get; set;}
    [JsonIgnore]
    public DateTime? EndsAt {get; set;}

}