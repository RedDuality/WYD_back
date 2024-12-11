
using FirebaseAdmin.Auth;

namespace Service;
public interface IAuthenticationService
{
    public Task<string> CheckTokenAsync(string token);
}