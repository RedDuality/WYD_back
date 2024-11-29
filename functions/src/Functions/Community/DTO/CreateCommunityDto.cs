using Model;

namespace Dto;

public class CreateCommunityDto
{
    public required string Name {get; set;}
    public CommunityType? Type { get; set; }

    public required HashSet<UserDto> Users { get; set; }
}