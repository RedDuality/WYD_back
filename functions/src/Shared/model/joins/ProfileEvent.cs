using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Model;

public enum EventRole{
    Owner,
    Viewer
}

[Table("Profile_Event")]
public class ProfileEvent
{
    [JsonIgnore]
    [ForeignKey("EventId")]
    public required virtual Event Event { get; set; }
    [JsonIgnore]
    [ForeignKey("ProfileId")]
    public required virtual Profile Profile { get; set; }

    public required EventRole Role { get; set; } = EventRole.Owner;

    public Boolean Confirmed { get ; set; } = false;
    public Boolean Trusted {get; set; } = false;

}