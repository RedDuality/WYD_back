

using Model;

namespace Dto;

public class EventDto
{
    public int Id {get; set;}
    public string? Hash {get; set;}
    public string? Title { get ; set; }
    public string? Description { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public int? GroupId { get; set; } = 0;
    public List<ProfileEventDto> ProfileEvents {get; set;}


    // Parameterized constructor for custom use
    public EventDto(Event ev)
    {
        Id = ev.Id;
        Hash = ev.Hash;
        Title = ev.Title;
        Description = ev.Description;
        StartTime = ev.StartTime;
        EndTime = ev.EndTime;
        GroupId = ev.Group != null ? ev.Group.Id : -1;
        ProfileEvents = ev.ProfileEvents.Select(pe => new ProfileEventDto(pe)).ToList();
    }

    // Parameterless constructor for deserialization
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EventDto()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

}