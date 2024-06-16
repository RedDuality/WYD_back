

using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Model;



namespace Functions
{
    public class ConfirmEvent
    {
        private readonly ILogger<ConfirmEvent> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public ConfirmEvent(ILogger<ConfirmEvent> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
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
            
            Event ev;
            try {
                ev = _eventController.Retrieve(id);
            } catch (Exception e) {
                return new NotFoundObjectResult(e.ToString());
            }
            _eventController.ConfirmEvent(ev, user);
            return new OkObjectResult("");

        }
    }
}
