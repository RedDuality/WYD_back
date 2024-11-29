using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


namespace Model;

public enum ProfileType {
    Personal,
    Business
}

[Table("Profiles")]
public class Profile : BaseEntity
{
    [NotNull]
    public ProfileType Type { get; set; } = ProfileType.Personal;

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserRole> UserRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<Event> Events { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileEvent> ProfileEvents { get; set; } = [];

}