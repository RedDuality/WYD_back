using Model;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace Controller;
public class AuthService
{


    private UserService _userService;
    private AccountService _accountService;

    public AuthService(UserService userService, AccountService accountService)
    {
        _userService = userService;
        _accountService = accountService;

        if (FirebaseApp.DefaultInstance == null)
        {
            var googleCredentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS");

            if (string.IsNullOrEmpty(googleCredentialsJson))
            {
                throw new Exception("Google credentials not found in the environment variable");
            }

            GoogleCredential credential;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(googleCredentialsJson)))
            {
                credential = GoogleCredential.FromStream(stream);
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = "wydaccounts",
            });
        }
    }

    public async Task<UserRecord> RetrieveFirebaseUser(String uid)
    {
        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        return userRecord;
    }

    private async Task<String> CheckFirebaseTokenAsync(String token)
    {
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        string uid = decodedToken.Uid;
        return uid;
    }

    public async Task<User> VerifyTokenAsync(String token)
    {
        String uid = "";
        try
        {
            uid = await this.CheckFirebaseTokenAsync(token);
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("Invalid Token");
        }

        Account? account = _accountService.Get(uid);
        
        if (account == null) //registration
        {
            UserRecord? UR = null;
            try
            {
                UR = await this.RetrieveFirebaseUser(uid);
            }
            catch (Exception)
            {
                throw new SecurityTokenValidationException("No Firebase user found");
            }
            return _userService.Create(UR);
        }

        if (account.User != null) //login
            return account.User;
        else //should prolly create a new user without too many questions
            throw new Exception("No user linked to the account");
    }


    public async Task<User> VerifyRequestAsync(HttpRequest req)
    {

        var authorization = req.Headers.Authorization;

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Bearer "))
        {
            throw new NullReferenceException("No token in the request");
        }

        string token = authorization.ToString().Substring("Bearer ".Length).Trim();

        try
        {
            string uid = await this.CheckFirebaseTokenAsync(token);
            return _userService.RetrieveFromAccountUid(uid);
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("Invalid Token");
        }
    }
}