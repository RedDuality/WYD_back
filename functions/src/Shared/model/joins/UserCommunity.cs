using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("User_Community")]
public class UserCommunity
{
    [ForeignKey("UserId")]
    public required virtual User User {get; set;}
    [ForeignKey("CommunityId")]
    public required virtual Community Community {get; set;}
}