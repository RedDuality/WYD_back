

using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class ConfirmEvent(ILogger<ConfirmEvent> logger, EventService eventService, AuthorizationService authorizationService)
    {
        private readonly ILogger<ConfirmEvent> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;


        [Function("ConfirmEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Confirm/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                _eventService.ConfirmOrDecline(int.Parse(eventId), currentProfile, true);
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
