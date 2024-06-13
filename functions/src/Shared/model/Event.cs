using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace Model;

[Table("Events")]
[Index(nameof(Hash), IsUnique = true)]
public class Event
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    public int? Hash {get; set;} = (DateTime.Now.Nanosecond + new Random().Next()).GetHashCode();
    public int? OwnerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool? IsAllDay{ get; set; } = false;
    public string? Color { get; set; }
    public string? StartTimeZone { get; set; }
    public string? EndTimeZone { get; set; }
    public string? RecurrenceRule { get; set; }
    public string? Notes { get; set; }
    public string? Location  { get; set; }
    public int? RecurrenceId{ get; set; }
    public string? Subject { get ; set; }

    [JsonIgnore]
    public virtual HashSet<User> Users { get; set;} = [];

    public virtual List<UserEvent> UserEvents {get; set;} = [];

    public void update(Event ev){
        StartTime = ev.StartTime;
        EndTime = ev.EndTime;
        IsAllDay = ev.IsAllDay;
        Color = ev.Color;
        StartTimeZone = ev.StartTimeZone;
        EndTimeZone = ev.EndTimeZone;
        RecurrenceRule = ev.RecurrenceRule;
        Notes = ev.Notes;
        RecurrenceId = ev.RecurrenceId;
        Subject = ev.Subject;
    }
}