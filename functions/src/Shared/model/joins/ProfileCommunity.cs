using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Profile_Community")]
public class ProfileCommunity
{
    [ForeignKey("ProfileId")]
    public required virtual Profile Profile {get; set;}
    [ForeignKey("CommunityId")]
    public required virtual Community Community {get; set;}
}