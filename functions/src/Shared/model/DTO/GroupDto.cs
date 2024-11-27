

using Model;

namespace Dto;


public class GroupDto(Group group)
{
    public int Id { get; set; } = group.Id;
    public string Hash { get; set; } = group.Hash;

    public string Name { get; set; } = group.Name;

    public bool GeneralForCommunity { get; set; } = group.GeneralForCommunity;

    public List<UserGroupDto> UserGroups { get; set; } = group.UserGroups.Select((ug) => new UserGroupDto(ug)).ToList();



}