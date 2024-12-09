using Model;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace Service;
public class AuthenticationService
{

    private readonly UserService _userService;

    public AuthenticationService(UserService userService)
    {
        _userService = userService;
    }

    private static FirebaseAuth GetInstance()
    {
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
        return FirebaseAuth.DefaultInstance;
    }

    public static async Task<UserRecord> RetrieveFirebaseUser(string uid)
    {
        try
        {
            UserRecord userRecord = await GetInstance().GetUserAsync(uid);
            return userRecord;
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("No Firebase user found");
        }

    }

    public static async Task<UserRecord> CreateUserAsync(UserRecordArgs userRecordArgs)
    {
        return await GetInstance().CreateUserAsync(userRecordArgs);
    }

    private static async Task<string> CheckFirebaseTokenAsync(string token)
    {
        FirebaseToken decodedToken = await GetInstance().VerifyIdTokenAsync(token);
        string uid = decodedToken.Uid;
        return uid;
    }

    public static async Task<UserRecord> RetrieveFirebaseUserFromMail(string mail)
    {
        return await GetInstance().GetUserByEmailAsync(mail);
    }


    public async Task<User> VerifyTokenAsync(string token)
    {
        string uid;
        try
        {
            uid = await CheckFirebaseTokenAsync(token);
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("Invalid Token");
        }

        return await _userService.GetOrCreateAsync(uid);
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
            string uid = await CheckFirebaseTokenAsync(token);
            return await _userService.GetOrCreateAsync(uid);
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("Invalid Token");
        }
    }

}