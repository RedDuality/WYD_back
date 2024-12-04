using System.ComponentModel.DataAnnotations.Schema;

namespace Model;

[Table("Profile_Group")]
public class ProfileGroup
{
    [ForeignKey("ProfileId")]
    public required virtual Profile Profile {get; set;}
    [ForeignKey("GroupId")]
    public required virtual Group Group {get; set;}
    public required bool Trusted { get; set; } = false; 
    public required long Color { get; set; } = 4278190080;//black
}