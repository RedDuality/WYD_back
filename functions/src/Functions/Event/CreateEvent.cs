

using System.Text;
using System.Text.Json;
using Controller;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class CreateEvent
    {
        private readonly ILogger<CreateEvent> _logger;
        private readonly EventService _eventService;
        private readonly AuthService _authService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CreateEvent(ILogger<CreateEvent> logger, AuthService authService, EventService eventService, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authService = authService;
            _eventService = eventService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Create")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authService.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonSerializer.Deserialize<EventDto>(requestBody, _jsonSerializerOptions);


            if (myevent != null)
            {
                var newevent = _eventService.Create(myevent, user.MainProfile!);
                return new OkObjectResult(newevent);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
