
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model;

namespace Service;
public class RequestService(JsonSerializerOptions jsonSerializerOptions, UserService userService, IAuthenticationService authenticationService)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    private readonly UserService _userService = userService;

    private readonly IAuthenticationService _authenticationService = authenticationService;


    public async Task<User> VerifyRequestAsync(HttpRequest req)
    {

        var authorization = req.Headers.Authorization;

        if (string.IsNullOrEmpty(authorization) || !authorization.ToString().StartsWith("Bearer "))
        {
            throw new NullReferenceException("No token in the request");
        }

        string token = authorization.ToString()["Bearer ".Length..].Trim();

        return await CheckTokenAsync(token);
    }


    public async Task<User> CheckTokenAsync(string token)
    {
        string uid;
        try
        {
            uid = await _authenticationService.CheckTokenAsync(token);
        }
        catch (Exception)
        {
            throw new SecurityTokenValidationException("Invalid Token");
        }
        //TODO check on maximum users from different domains
        return await _userService.GetOrCreateAsync(uid);
    }


    public async Task<T> DeserializeRequestBodyAsync<T>(HttpRequest req)
    {
        string requestBody;
        using (StreamReader reader = new(req.Body, Encoding.UTF8))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        return JsonSerializer.Deserialize<T>(requestBody, _jsonSerializerOptions) ?? throw new ArgumentNullException(nameof(T));
    }

    public static IActionResult GetErrorResult(Exception e)
    {
        return e switch
        {
            InvalidOperationException invalidOperationException => new BadRequestObjectResult(invalidOperationException.Message),
            UnauthorizedAccessException unauthorizedEx => new StatusCodeResult(StatusCodes.Status403Forbidden),
            SecurityTokenValidationException securityTokenValidationException => new UnauthorizedObjectResult(securityTokenValidationException.Message),
            NullReferenceException nullReferenceException => new BadRequestObjectResult(nullReferenceException.Message),
            KeyNotFoundException keyNotFoundEx => new BadRequestObjectResult(keyNotFoundEx.Message + " with given Id not found"),
            ArgumentNullException argumentNullException => new BadRequestObjectResult("Expected a value but none was given"),
            FormatException formatEx => new BadRequestObjectResult("Id Format wrong"),
            OverflowException overflowEx => new BadRequestObjectResult("Id Format wrong"),
            ArgumentException argumentException => new BadRequestObjectResult(argumentException.Message),
            JsonException jsonException => new BadRequestObjectResult(jsonException.Message),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError),
        };
    }

}