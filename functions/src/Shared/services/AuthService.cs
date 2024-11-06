using Model;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace Controller;
public class AuthService
{

    
    private UserService _userService;

    

    private readonly SymmetricSecurityKey _secretKey;

    public AuthService(UserService userService)
    {
        
        _userService = userService;

        var secret = Environment.GetEnvironmentVariable("LoginTokenSecret") ?? throw new Exception();
        _secretKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));


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

    public async Task<UserRecord> retrieveFirebaseUser(String uid)
    {
        UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        return userRecord;
    }

    public async Task<String> VerifyTokenAsync(String token)
    {
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        string uid = decodedToken.Uid;
        return uid;
    }

    public async Task<User> VerifyTokenAndCreateUserAsync(String token)
    {
        string uid = await this.VerifyTokenAsync(token);
        UserRecord UR = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

        User savedUser = _userService.Create(UR);

        return savedUser;
    }


    public async Task<User> VerifyRequestAsync(HttpRequest req)
    {

        var authorization = req.Headers.Authorization;

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Bearer "))
        {
            throw new NullReferenceException("No token in the request");
        }

        string token = authorization.ToString().Substring("Bearer ".Length).Trim();

        try{
            String uid = await this.VerifyTokenAsync(token);
            return _userService.RetrieveFromAccountUid(uid);
        } catch (Exception ) {
            throw new SecurityTokenValidationException("Invalid Token");
        }
    }
}