

namespace Model;



public class UserDto
{
    public int Id { get; set; }
    public string? Mail { get; set; }
    public string? Username { get; set; }
    public List<Community> Communities { get; set; }
    public List<Event> Events { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Mail = user.Mail;
        Username = user.Username;
        Communities = user.Communities;
        Events = user.Events;
    }

}