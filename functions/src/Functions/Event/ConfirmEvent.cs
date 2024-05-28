

using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class ConfirmEvent
    {
        private readonly ILogger<ConfirmEvent> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public ConfirmEvent(ILogger<ConfirmEvent> logger, EventController eventController, AuthController authController)
        {
            _logger = logger;
            _eventController = eventController;
            _authController = authController;
        }

        [Function("ConfirmEvent")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Confirm/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
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

            _eventController.ConfirmEvent(id, user);
            return new OkObjectResult("");


        }
    }
}
