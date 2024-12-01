using Model;

namespace Dto;

public class UserDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Tag { get; set; }
    public int MainProfileId {get; set;}

    public UserDto(User user){
        Id = user.Id;
        UserName = user.UserName;
        Tag = user.Tag;
        MainProfileId = user.MainProfileId ?? 0;
    }
    public UserDto (){

    }
}