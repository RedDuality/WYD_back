

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class RetrieveByHash(ILogger<RetrieveByHash> logger, EventService eventService, AuthorizationService authService)
    {
        private readonly ILogger<RetrieveByHash> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authService;

        [Function("RetrieveByHash")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Retrieve/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                Event e = _eventService.RetrieveByHash(eventHash);
                return new OkObjectResult(new EventDto(e));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
