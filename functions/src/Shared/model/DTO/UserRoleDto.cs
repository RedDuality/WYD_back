
using Model;

namespace Dto;



public class UserRoleDto(UserRole ur)
{

    public int Id = ur.Id;
    public Profile Profile { get; set; } = ur.Profile;
    public Role Role { get; set; } = ur.Role;

}