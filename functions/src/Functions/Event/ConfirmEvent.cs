

using Service;
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
        private readonly EventService _eventService;
        private readonly AuthenticationService _authController;

        public ConfirmEvent(ILogger<ConfirmEvent> logger, EventService eventService, AuthenticationService authService)
        {
            _logger = logger;
            _eventService = eventService;
            _authController = authService;
        }

        [Function("ConfirmEvent")]
        public async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Confirm/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
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

            Event? ev = _eventService.RetrieveOrNull(id);
            if (ev == null)
                return new NotFoundObjectResult("");
            try
            {
                _eventService.Confirm(ev, user.MainProfile);
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
