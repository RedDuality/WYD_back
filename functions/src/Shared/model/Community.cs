using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Dto;
using Service;

namespace Model;

public enum CommunityType
{
    Personal,
    SingleGroup,
    Community
}

[Table("Communities")]
public class Community : BaseEntity
{
    public string Name { get; set; } = "";

    public CommunityType Type { get; set; } = CommunityType.Personal;

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set; } = [];
    [JsonIgnore]
    public virtual List<UserCommunity> UserCommunities { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<Group> Groups { get; set; } = [];

}