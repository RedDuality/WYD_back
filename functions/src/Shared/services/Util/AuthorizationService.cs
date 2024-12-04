using Microsoft.AspNetCore.Http;
using Model;

namespace Service;
public class AuthorizationService(AuthenticationService authService, ProfileService profileService)
{
    private readonly AuthenticationService _authService = authService;
    private readonly ProfileService _profileService = profileService;

    public async Task<User> VerifyRequest(HttpRequest req)
    {
        try
        {
            return await _authService.VerifyRequestAsync(req);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException();
        }
    }

    public async Task<User> VerifyRequest(HttpRequest req, string requestedPermit)
    {
        try
        {
            return await _authService.VerifyRequestAsync(req);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException();
        }
    }

    public async Task<Profile> VerifyRequest(HttpRequest req, int profileId, string role)
    {
        try
        {
            await _authService.VerifyRequestAsync(req);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException();
        }
        return _profileService.Retrieve(profileId);
    }
}