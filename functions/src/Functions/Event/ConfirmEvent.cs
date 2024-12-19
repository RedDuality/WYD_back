

using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class ConfirmEvent(ILogger<ConfirmEvent> logger, EventService eventService, AuthorizationService authorizationService, RequestService requestService)
    {
        private readonly ILogger<ConfirmEvent> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;

        private readonly RequestService requestService = requestService;

        [Function("ConfirmEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Confirm/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                Event ev = _eventService.ConfirmOrDecline(eventHash, currentProfile, true);
                
                await requestService.NotifyAsync(ev, UdpateType.ConfirmEvent,currentProfile, req);
                
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
