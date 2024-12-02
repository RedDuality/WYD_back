

using Model;

namespace Dto;


public class GroupDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Hash { get; set; }
    public long Color {get; set; } = 4278190080; //black
    public bool? Trusted {get; set; } = false;
    public bool? GeneralForCommunity { get; set; }
    public HashSet<UserDto> Users {get; set; } =  [];


    public GroupDto(Group group, int currentUserId)
    {
        Id = group.Id;
        Name = group.Name;
        Hash = group.Hash;
        GeneralForCommunity = group.GeneralForCommunity;
        Users = group.UserGroups.Select((ug) => new UserDto(ug.User)).ToHashSet();

        var userGroup = group.UserGroups.FirstOrDefault(ug => ug.User.Id == currentUserId);
        if (userGroup != null)
        {
            Color = userGroup.Color;
            Trusted = userGroup.Trusted;
        }
    }

    public GroupDto(){
    }
}