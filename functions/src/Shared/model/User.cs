
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Model;


[Table("Users")]
[Index(nameof(MainMail), IsUnique = true)]
[Index(nameof(Tag), IsUnique = true)]//check onModelCreating, unique when not null
public class User : BaseHashEntity
{
    public string MainMail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;

    public int? MainProfileId { get; set; }

    [ForeignKey("MainProfileId")]
    public virtual Profile? MainProfile { get; set; }

    public virtual List<Account> Accounts { get; set; } = [];
    [JsonIgnore]
    public virtual List<Profile> Profiles { get; set; } = [];

    public virtual List<UserRole> UserRoles { get; set; } = [];

    [JsonIgnore]
    public virtual HashSet<Community> Communities { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserCommunity> UserCommunities { get; set; } = [];

    [JsonIgnore]
    public virtual HashSet<Group> Groups { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserGroup> UserGroups { get; set; } = [];
}