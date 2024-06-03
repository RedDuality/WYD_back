

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
    public class ShareEvent
    {
        private readonly ILogger<ShareEvent> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public ShareEvent(ILogger<ShareEvent> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("ShareEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Share/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            User user;
            try{
                user = _authController.VerifyRequest(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

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
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var userIdList = JsonSerializer.Deserialize<List<int>>(requestBody);

            if (userIdList != null)
            {
                var newevent = _eventController.Share(id, userIdList);
                return new OkObjectResult(newevent);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
