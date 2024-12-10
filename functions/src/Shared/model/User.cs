
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model;


[Table("Users")]
public class User : BaseHashEntity
{
    public int? MainProfileId { get; set; }

    [ForeignKey("MainProfileId")]
    public virtual Profile? MainProfile { get; set; }

    public virtual List<Account> Accounts { get; set; } = [];
    [JsonIgnore]
    public virtual List<Profile> Profiles { get; set; } = [];

    public virtual List<UserRole> UserRoles { get; set; } = [];


}