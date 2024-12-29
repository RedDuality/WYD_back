using Model;

namespace Dto;

public class RetrieveUserDto(User user)
{
    public int Id { get; set; } = user.Id;
    public string? Hash { get; set; } = user.Hash;
    public string MainProfileHash { get; set; } = user.MainProfile!.Hash;

    public List<AccountDto> Accounts { get; set; } = user.Accounts.Select(account => new AccountDto(account)).ToList();
    public List<UserRoleDto> UserRoles { get; set; } = user.UserRoles.Select(ur => new UserRoleDto(ur)).ToList();
    
}