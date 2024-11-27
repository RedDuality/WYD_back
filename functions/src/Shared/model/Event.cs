
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Dto;
using Microsoft.EntityFrameworkCore;

namespace Model;

[Table("Events")]
[Index(nameof(Hash), IsUnique = true)]
public class Event : BaseEntity
{
    [ForeignKey("ParentId")]
    public virtual Event? Parent { get; set; } = null;


    public string Hash { get; set; } = CreateHashCode();
    public string? Title { get; set; }
    public string? Description { get; set; }
    required public DateTimeOffset StartTime { get; set; }
    required public DateTimeOffset EndTime { get; set; }
    [ForeignKey("GroupId")]
    public virtual Group? Group { get; set; }

    [JsonIgnore]
    public virtual HashSet<Profile> Profiles { get; set; } = [];
    [JsonIgnore]
    public virtual ICollection<ProfileEvent> ProfileEvents { get; set; } = [];


    private static string CreateHashCode()
    {
        byte[] randomBytes = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(randomBytes);
        string result = Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_') 
            .Replace("=", "");
        return result;
    }
    
    static public Event FromDto(EventDto dto)
    {
        return new Event
        {
            Id = dto.Id,
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