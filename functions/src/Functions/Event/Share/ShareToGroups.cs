


using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;
using Dto;


namespace Functions
{
    public class ShareToGroups(ILogger<ShareToGroups> logger, EventService eventService, AuthorizationService authorizationService, RequestService requestService)
    {
        private readonly ILogger<ShareToGroups> _logger = logger;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;
        private readonly RequestService _requestService = requestService;

        [Function("ShareToGroups")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Share/Groups/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                var groupIds = await _requestService.DeserializeRequestBodyAsync<HashSet<int>>(req);

                //TODO check logic
                var newevent = _eventService.ShareToGroups(eventHash, groupIds!);
                

                await _requestService.NotifyAsync(newevent, UpdateType.ShareEvent, currentProfile);
                return new OkObjectResult(new EventDto(newevent));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
