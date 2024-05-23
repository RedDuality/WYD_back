
namespace Model;



public class UserDto
{
    public int Id { get; set; }
    public string? mail { get; set; }
    public string? username { get ; set; }
    public List<EventDto> Events {get; set;} = [];

}