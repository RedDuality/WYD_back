
using Model;

namespace Dto;



public class UserRoleDto
{

    public int Id;
    public Profile? Profile {get; set;}
       
    public Role Role { get; set; }

    
    //public DateTime? StartsAt {get; set;}
    
    //public DateTime? EndsAt {get; set;}
    public UserRoleDto(UserRole ur)
    {
        Id = ur.Id;
        Profile = ur.Profile;
        Role = ur.Role;
    }
}