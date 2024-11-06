using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Model;

namespace Dto;


public class EventDto
{
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
    public int? Id { get; set; }
    public string? subject { get ; set; }

    public List<UserEventDto> UserEvents {get; set;} = [];
}