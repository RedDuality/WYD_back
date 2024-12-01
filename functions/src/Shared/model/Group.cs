
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model;


[Table("Groups")]
public class Group : BaseHashEntity
{
    public string Name { get; set; } = "";

    public bool GeneralForCommunity {get; set; } = true;

    [ForeignKey("CommunityId")]
    public virtual required Community Community { get; set; }

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserGroup> UserGroups { get; set; } = [];
}