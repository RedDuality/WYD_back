

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class RetrieveFromHash
    {
        private readonly ILogger<RetrieveFromHash> _logger;
        private readonly EventService _eventService;
        private readonly AuthorizationService _authorizationService;

        public RetrieveFromHash(ILogger<RetrieveFromHash> logger, EventService eventService, AuthorizationService authService)
        {
            _logger = logger;
            _eventService = eventService;
            _authorizationService = authService;
        }

        [Function("RetrieveFromHash")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Retrieve/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                Event e = _eventService.RetrieveFromHash(eventHash, currentProfile);
                return new OkObjectResult(new EventDto(e));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
