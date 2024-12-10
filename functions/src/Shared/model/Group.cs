
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Model;


[Table("Groups")]
public class Group : BaseHashEntity
{
    public string Name { get; set; } = "";

    public string? BlobHash { get; set;}

    public bool GeneralForCommunity {get; set; } = true;

    [ForeignKey("CommunityId")]
    public virtual required Community Community { get; set; }

    [JsonIgnore]
    public virtual HashSet<Profile> Profiles { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileGroup> ProfileGroups { get; set; } = [];

}