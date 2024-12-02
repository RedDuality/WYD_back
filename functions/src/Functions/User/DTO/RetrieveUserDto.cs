using Model;

namespace Dto;

public class RetrieveUserDto
{
    public int Id { get; set; }
    public string? Hash {get; set;}
    public string? Tag { get; set; } = string.Empty;
    public string? MainMail { get; set; }
    public string? UserName { get; set; }

    public int MainProfileId { get; set; }

    public List<AccountDto> Accounts { get; set; } = [];
    public List<UserRoleDto> UserRoles { get; set; } = [];

    public List<CommunityDto> Communities {get; set;} = [];

    public RetrieveUserDto(User user)
    {
        Id = user.Id;
        Hash = user.Hash;
        MainMail = user.MainMail;
        UserName = user.UserName;
        Tag = user.Tag;
        Accounts = user.Accounts.Select(account => new AccountDto(account)).ToList();
        UserRoles = user.UserRoles.Select(ur => new UserRoleDto(ur)).ToList();
        MainProfileId = user.MainProfile!.Id;
        Communities = user.Communities.Select(c => new CommunityDto(c, user.Id)).ToList();
    }

}