

using Model;

namespace Dto;

public class EventDto(Event ev)
{
    public int Id {get; set;} = ev.Id;
    public string? Hash {get; set;} = ev.Hash;
    public string? Title { get ; set; } = ev.Title;
    public string? Description { get; set; } = ev.Description;
    public DateTime StartTime { get; set; } = ev.StartTime;
    public DateTime EndTime { get; set; } = ev.EndTime;
    public int? GroupId { get; set; } = ev.Group != null ? ev.Group.Id : -1;
    public virtual List<ProfileEventDto> ProfileEvents {get; set;} = ev.ProfileEvents.Select(pe => new ProfileEventDto(pe)).ToList();

}