using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace Model;

[Table("User_Event")]
public class UserEvent
{
    [JsonIgnore]
    public int EventId { get; set; }
    public int UserId { get; set; }
    public bool confirmed { get ; set; } = false;
}