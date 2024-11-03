using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("User_Group")]
public class UserGroup : BaseEntity
{
    [ForeignKey("UserId")]
    public required virtual User User {get; set;}
    [ForeignKey("GroupId")]
    public required virtual Group Group {get; set;}
    
    public required Boolean Trusted { get; set; } = false; 
    public required string Color { get; set; }
}