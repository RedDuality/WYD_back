

using System.Text;
using System.Text.Json;
using Controller;
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
        private readonly EventService _eventController;
        private readonly AuthService _authController;
        private readonly UserService _userController;

        public CreateEvent(ILogger<CreateEvent> logger, AuthService authService, EventService eventService, UserService userService)
        {
            _logger = logger;
            _authController = authService;
            _eventController = eventService;
            _userController = userService;
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try{
                user = _authController.VerifyRequest(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonSerializer.Deserialize<Event>(requestBody);


            if (myevent != null)
            {
                User uc = _userController.Get(user.Id);
                var newevent = _eventController.Create(myevent, uc);
                string result = JsonSerializer.Serialize(newevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
