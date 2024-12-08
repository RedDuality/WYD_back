
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
        return JsonSerializer.Deserialize<T>(requestBody, _jsonSerializerOptions) ?? throw new ArgumentNullException("");
    }



    public static IActionResult GetErrorResult(Exception e)
    {
        return e switch
        {
            UnauthorizedAccessException unauthorizedEx => new StatusCodeResult(StatusCodes.Status403Forbidden),
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