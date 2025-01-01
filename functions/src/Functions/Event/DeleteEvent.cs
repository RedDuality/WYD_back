


using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class DeleteEvent(ILogger<DeleteEvent> logger, AuthorizationService authorizationService, EventService eventService, RequestService requestService)
    {
        private readonly ILogger<DeleteEvent> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly RequestService requestService = requestService;
        private readonly AuthorizationService _authorizationService = authorizationService;

        [Function("DeleteEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/Delete/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.DELETE_EVENT);

                var ev = _eventService.DeleteForProfile(eventHash, currentProfile);
                
                //Notify
                //TODO
                await requestService.NotifyAsync(ev, UpdateType.DeleteEvent, currentProfile, eventHash);
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
