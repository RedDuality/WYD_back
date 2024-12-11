using Model;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace Service;
public class AuthenticationService(UserService userService) : IAuthenticationService
{

    private readonly UserService _userService = userService;

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

    public async Task<string> CheckTokenAsync(string token)
    {
        FirebaseToken decodedToken = await GetInstance().VerifyIdTokenAsync(token);
        string uid = decodedToken.Uid;
        return uid;
    }


    //Util
    public static async Task<UserRecord> CreateUserAsync(UserRecordArgs userRecordArgs)
    {
        return await GetInstance().CreateUserAsync(userRecordArgs);
    }

    public static async Task<UserRecord> RetrieveFirebaseUserFromMail(string mail)
    {
        return await GetInstance().GetUserByEmailAsync(mail);
    }

}