using Model;
using dto;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Controller;
public class AuthService
{

    private UserService _userController;

    private readonly SymmetricSecurityKey _secretKey;

    public AuthService(UserService userService){
        _userController = userService;

        var secret = Environment.GetEnvironmentVariable("LoginTokenSecret") ?? throw new Exception();
        _secretKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
    }

    public string Register(LoginDto registerDto)
    {
        CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
        User user = new User
        {
            
            Username = registerDto.Username,
            Mail = registerDto.Mail,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        var newUser = _userController.Create(user);
        return CreateToken(newUser);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

    }

    public bool Login(LoginDto loginDto, out string token)
    {
        User user = _userController.RetrieveByMail(loginDto.Mail);
        token = string.Empty;
        if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            return false;

        token = CreateToken(user);
        return true;
    }


    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Mail),
            new Claim(ClaimTypes.Role,"User")
        };
        
        var cred = new SigningCredentials(_secretKey, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
                               claims: claims,
                               expires: DateTime.UtcNow.AddDays(1),
                               signingCredentials: cred
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }


    public User VerifyRequest(HttpRequest req){

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

            if(mail == null)
                throw new Exception("Mail non valida");

            // You can process the claims or return them as needed
            return _userController.RetrieveByMail(mail.Value);
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