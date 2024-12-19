

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class SharedWithHash(ILogger<SharedWithHash> logger, EventService eventService, AuthorizationService authService, RequestService requestService)
    {
        private readonly ILogger<SharedWithHash> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authService;

        private readonly RequestService requestService = requestService;


        [Function("SharedWithHash")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Shared/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                Event e = _eventService.SharedWithHash(eventHash, currentProfile);
                await requestService.NotifyAsync(e, UdpateType.NewEvent, currentProfile, req);
                return new OkObjectResult(new EventDto(e));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
