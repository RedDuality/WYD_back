using Model;

namespace Dto;

public class UserDto
{
    public int Id { get; set; }
    public string? Mail { get; set; }
    public string? Username { get; set; }

    public UserDto(User user)
    {
        Id = (int)user.Id;
        Mail = user.MainMail;
        Username = user.UserName;
    }

}