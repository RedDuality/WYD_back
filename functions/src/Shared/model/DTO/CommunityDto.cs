


using Model;

namespace Dto;


public class CommunityDto(Community community)
{
    public int Id {get; set; } = community.Id;
    public string Name { get; set; } = community.Name;

    public CommunityType Type { get; set; } = community.Type;

    public ICollection<GroupDto> Groups { get; set; } = community.Groups.Select((g) => new GroupDto(g)).ToList();
}