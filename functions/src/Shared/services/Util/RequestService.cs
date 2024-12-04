
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service;
public class RequestService(JsonSerializerOptions jsonSerializerOptions)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

    public async Task<T> DeserializeRequestBodyAsync<T>(HttpRequest req)
    {
        string requestBody;
        using (StreamReader reader = new(req.Body, Encoding.UTF8))
        {
            requestBody = await reader.ReadToEndAsync();
        }
        return JsonSerializer.Deserialize<T>(requestBody, _jsonSerializerOptions) ?? throw new ArgumentNullException();
    }



    public static IActionResult GetErrorResult(Exception e)
    {
        switch (e)
        {
            case UnauthorizedAccessException unauthorizedEx:
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
                
            case KeyNotFoundException keyNotFoundEx:
                return new BadRequestObjectResult(keyNotFoundEx.Message+" with given Id not found");

            case ArgumentNullException argumentNullException:
                return new BadRequestObjectResult("Expected a value but none was given");

            case FormatException formatEx:
                return new BadRequestObjectResult("Id Format wrong");

            case OverflowException overflowEx:
                return new BadRequestObjectResult("Id Format wrong");

            default:
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}