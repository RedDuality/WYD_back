using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;


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
    [ForeignKey("UserId")]
    public required virtual User User { get; set; }

    public required EventRole eventRole { get; set; } = EventRole.Viewer;

    public Boolean Confirmed { get ; set; } = false;
    public Boolean Trusted {get; set; } = false;

}