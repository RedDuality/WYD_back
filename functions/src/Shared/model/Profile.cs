using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace Model;

public enum ProfileType
{
    Personal,
    Shared
}

[Table("Profiles")]
[Index(nameof(Tag), IsUnique = true)]//check onModelCreating, unique when not empty
public class Profile : BaseHashEntity
{
    [NotNull]
    public ProfileType Type { get; set; } = ProfileType.Personal;
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string? ImageHash { get; set; }

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserRole> UserRoles { get; set; } = [];

    [JsonIgnore]
    public virtual HashSet<Community> Communities { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileCommunity> UserCommunities { get; set; } = [];

    [JsonIgnore]
    public virtual HashSet<Group> Groups { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileGroup> UserGroups { get; set; } = [];

    [JsonIgnore]
    public virtual List<Event> Events { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileEvent> ProfileEvents { get; set; } = [];

}