

using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Model;



namespace Functions
{
    public class CreateEvent
    {
        private readonly ILogger<CreateEvent> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;
        private readonly UserService _userController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CreateEvent(ILogger<CreateEvent> logger, AuthService authService, EventService eventService, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authController = authService;
            _eventController = eventService;
            _userController = userService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Create")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonSerializer.Deserialize<Event>(requestBody, _jsonSerializerOptions);


            if (myevent != null)
            {
                User uc = _userController.Get(user.Id);
                var newevent = _eventController.Create(myevent, uc);
                return new OkObjectResult(newevent);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
