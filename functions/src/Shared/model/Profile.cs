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
public class Profile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [NotNull]
    public ProfileType Type { get; set; } = ProfileType.Personal;


    [JsonIgnore]
    public virtual List<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserRole> UserRoles { get; set; } = [];

    [JsonIgnore]
    public virtual List<Event> Events { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileEvent> ProfileEvents { get; set; } = [];




}