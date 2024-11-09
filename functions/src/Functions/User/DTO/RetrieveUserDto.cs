using Model;

namespace Dto;

public class RetrieveUserDto
{
    public int Id { get; set; }
    public string? Uid {get; set;}
    public string? Tag { get; set; } = string.Empty;
    public string? MainMail { get; set; }
    public string? UserName { get; set; }

    public List<Account> Accounts { get; set; } = [];
    public List<UserRoleDto> UserRoles { get; set; } = [];

    public RetrieveUserDto(User user)
    {
        Id = (int)user.Id;
        Uid = user.Uid;
        MainMail = user.MainMail;
        UserName = user.UserName;
        Tag = user.Tag;
        Accounts = user.Accounts;
        UserRoles = user.UserRoles.Select(ur => new UserRoleDto(ur)).ToList();
    }

}