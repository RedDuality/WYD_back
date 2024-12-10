

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class UpdateEvent(ILogger<UpdateEvent> logger, AuthorizationService authorizationService, RequestService requestService, EventService eventService)
    {
        private readonly ILogger<UpdateEvent> _logger = logger;
        private readonly RequestService _requestService = requestService;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;


        [Function("UpdateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Update")] HttpRequest req, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.UPDATE_EVENT);

                var myevent = await _requestService.DeserializeRequestBodyAsync<EventDto>(req);
                var newevent = await _eventService.UpdateAsync(myevent);

                EventDto eventDto = new(newevent);

                return new OkObjectResult(eventDto);
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
