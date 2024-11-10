


using Model;

namespace Dto;


public class ProfileEventDto(ProfileEvent pe)
{
    public int ProfileId { get; set; } = pe.Profile.Id;

    public EventRole Role { get; set; } = pe.Role;

    public bool Confirmed { get ; set; } = pe.Confirmed;
    public bool Trusted {get; set; } = pe.Trusted;

}