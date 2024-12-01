

using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class DeclineEvent
    {
        private readonly ILogger<DeclineEvent> _logger;
        private readonly EventService _eventService;
        private readonly AuthService _authController;

        public DeclineEvent(ILogger<DeclineEvent> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventService = eventService;
            _authController = authService;
        }

        [Function("DeclineEvent")]
        public async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Decline/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authController.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

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
            try
            {
                ev = _eventService.Retrieve(id);
            }
            catch (Exception e)
            {
                return new NotFoundObjectResult(e.ToString());
            }

            _eventService.Decline(ev, user.MainProfile);
            return new OkObjectResult("");


        }
    }
}
