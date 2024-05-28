using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;


namespace Model;

[Table("Events")]
public class Event
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    public int? OwnerId {get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
    public bool? isAllDay{ get; set; } = false;
    public string? color { get; set; }
    public string? startTimeZone { get; set; }
    public string? endTimeZone { get; set; }
    public string? recurrenceRule { get; set; }
    public string? notes { get; set; }
    public string? location  { get; set; }
    public int? recurrenceId{ get; set; }

    [NotMapped]
    public string? link { get; set; }
    public string? subject { get ; set; }


    [JsonIgnore]
    public virtual HashSet<User> users { get; set;} = [];

    public virtual List<UserEvent> UserEvents {get; set;} = [];

    public void update(Event ev){
        startTime = ev.startTime;
        endTime = ev.endTime;
        isAllDay = ev.isAllDay;
        color = ev.color;
        startTimeZone = ev.startTimeZone;
        endTimeZone = ev.endTimeZone;
        recurrenceRule = ev.recurrenceRule;
        notes = ev.notes;
        recurrenceId = ev.recurrenceId;
        subject = ev.subject;
    }
}