


using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class DeleteEvent(ILogger<DeleteEvent> logger, AuthorizationService authorizationService)
    {
        private readonly ILogger<DeleteEvent> _logger = logger;
        //private readonly EventService _eventService;

        private readonly AuthorizationService _authorizationService = authorizationService;

        [Function("DeleteEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/Delete/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.DELETE_EVENT);

                //var result = _eventService.DeleteForUser(int.Parse(eventId), user.Id);
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
