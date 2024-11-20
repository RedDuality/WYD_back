using Model;

namespace Dto;

public class ProfileEventDto
{
    public int ProfileId { get; set; }
    public EventRole Role { get; set; }
    public bool Confirmed { get; set; }
    public bool Trusted { get; set; }

    // Parameterized constructor for custom initialization
    public ProfileEventDto(ProfileEvent pe)
    {
        ProfileId = pe.Profile.Id;
        Role = pe.Role;
        Confirmed = pe.Confirmed;
        Trusted = pe.Trusted;
    }

    // Parameterless constructor for deserialization
    public ProfileEventDto() { }
}
