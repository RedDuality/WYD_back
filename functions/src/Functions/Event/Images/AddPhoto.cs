using System.Text;
using System.Text.Json;
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class AddPhoto
    {
        private readonly ILogger<AddPhoto> _logger;
        private readonly EventService _eventService;
        private readonly AuthenticationService _authService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public AddPhoto(ILogger<AddPhoto> logger, AuthenticationService authService, EventService eventService, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authService = authService;
            _eventService = eventService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("AddPhoto")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Photo/Add/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            /*
            User user;
            try
            {
                user = await _authService.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }
*/
            int id;
            try
            {
                id = Int32.Parse(eventId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }
            string requestBody;

            using (StreamReader reader = new(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            //var myevent = JsonSerializer.Deserialize<EventDto>(requestBody, _jsonSerializerOptions);


            await _eventService.AddImageAsync(id);
            return new OkObjectResult("");

        }
    }
}
