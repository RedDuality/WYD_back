using Model;

namespace Dto;

public class UserDto
{
    public int Id { get; set; }
    public string? Mail { get; set; }
    public string? UserName { get; set; }

    public UserDto(User user)
    {
        Id = (int)user.Id;
        Mail = user.MainMail;
        UserName = user.UserName;
    }

}