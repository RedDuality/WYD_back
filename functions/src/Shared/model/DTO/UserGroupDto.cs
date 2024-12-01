
using Model;

namespace Dto;


public class UserGroupDto(UserGroup ug)
{
    public UserDto User { get; set; } = new UserDto(ug.User);
    public bool Trusted { get; set; } = ug.Trusted; 
    public long Color { get; set; } = ug.Color;
}