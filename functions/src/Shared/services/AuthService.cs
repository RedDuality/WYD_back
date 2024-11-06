using Model;
using dto;
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

    private AccountService _accountService;
    private UserService _userService;

    private ProfileService _profileService;

    private readonly SymmetricSecurityKey _secretKey;

    public AuthService(UserService userService, AccountService accountService, ProfileService profileService)
    {
        _accountService = accountService;
        _userService = userService;
        _profileService = profileService;

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
        UserRecord user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        return user;
    }

    public async Task<String> VerifyTokenAsync(String token)
    {
        FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        string uid = decodedToken.Uid;
        return uid;
    }

    public async Task<UserRecord> VerifyTokenAndCreateAsync(String token)
    {
        string uid = await this.VerifyTokenAsync(token);
        UserRecord UR = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);


        
        User user = new ();
        user.MainMail = UR.Email;

        User savedUser = _userService.Create(user);
       

        Account account = new()
        {
            Mail = UR.Email,
            Uid = UR.Uid,
            User = savedUser
        };

        _accountService.Create(account);


        Profile profile = new();
        profile.Users.Add(user);

        _profileService.Create(profile);

        return UR;
    }





    public User VerifyRequest(HttpRequest req)
    {

        var authorization = req.Headers.Authorization;

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Bearer "))
        {
            throw new NullReferenceException("No token in the request");
        }

        string token = authorization.ToString().Substring("Bearer ".Length).Trim();

        if (ValidateToken(token, out SecurityToken validatedToken))
        {
            // Token is valid, proceed with your logic
            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims;


            var mail = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (mail == null)
                throw new Exception("Mail non valida");

            // You can process the claims or return them as needed
            //return _userController.RetrieveByMail(mail.Value);
            return null;
        }
        else
        {
            // Token is invalid
            throw new SecurityTokenValidationException("Invalid Token");
        }
    }

    private bool ValidateToken(string token, out SecurityToken validatedToken)
    {

        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _secretKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // Optional: adjust clock skew tolerance if needed
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true;
        }
        catch
        {
            validatedToken = new JwtSecurityToken();
            // Token validation failed
            return false;
        }
    }



}