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

    public async Task<Profile> VerifyRequest(HttpRequest req, UserPermissionOnProfile permissionOnProfile)
    {
        try
        {
            await _authService.VerifyRequestAsync(req);
        }
        catch (Exception)
        {
            throw new UnauthorizedAccessException();
        }
        //TODO check roles
        return _profileService.RetrieveFromHeaders(req);
    }
}