
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Model;

[Table("Events")]
[Index(nameof(Hash), IsUnique = true)]
public class Event : BaseEntity
{
    [ForeignKey("ParentId")]
    public virtual Event? Parent {get; set;} = null;
    public string Hash {get; set;} = Convert.ToBase64String(BitConverter.GetBytes(DateTime.Now.GetHashCode() * new Random().NextInt64()));
    public string? Title { get ; set; }
    public string? Description { get; set; }
    required public DateTime StartTime { get; set; }
    required public DateTime EndTime { get; set; }
    [ForeignKey("GroupId")]
    public virtual Group? Group { get; set; }

    [JsonIgnore]
    public virtual HashSet<Profile> Profiles { get; set;} = [];
    public virtual List<ProfileEvent> ProfileEvents {get; set;} = [];

}