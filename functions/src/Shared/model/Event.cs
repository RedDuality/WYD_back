
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Dto;

namespace Model;

[Table("Events")]
public class Event : BaseHashEntity
{
    [ForeignKey("ParentId")]
    public virtual Event? Parent { get; set; } = null;
    public string? Title { get; set; }
    public string? Description { get; set; }
    required public DateTimeOffset StartTime { get; set; }
    required public DateTimeOffset EndTime { get; set; }
    [ForeignKey("GroupId")]
    public virtual Group? Group { get; set; }

    [JsonIgnore]
    public virtual HashSet<Profile> Profiles { get; set; } = [];
    [JsonIgnore]
    public virtual List<ProfileEvent> ProfileEvents { get; set; } = [];

    public virtual ICollection<Blob> Blobs { get; set; } = [];

    static public Event FromDto(EventDto dto)
    {
        return new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
        };
    }

    public void Update(EventDto dto){
        Title = dto.Title ?? Title;
        Description = dto.Description ?? Description;
        StartTime = dto.StartTime;
        EndTime = dto.EndTime;
    }


}