using Model;

namespace Dto;

public class UserDto(User user)
{
    public int Id { get; set; } = user.Id;
    public string? Mail { get; set; } = user.MainMail;
    public string? UserName { get; set; } = user.UserName;
}