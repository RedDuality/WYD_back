

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class CreateEvent(ILogger<CreateEvent> logger, AuthorizationService authorizationService, RequestService requestService, EventService eventService)
    {
        private readonly ILogger<CreateEvent> _logger = logger;
        private readonly RequestService _requestService = requestService;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Create")] HttpRequest req, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CREATE_EVENT);

                var myevent = await _requestService.DeserializeRequestBodyAsync<EventDto>(req);
                var newevent = await _eventService.Create(myevent, currentProfile);

                await _requestService.NotifyAsync(newevent, UdpateType.NewEvent, currentProfile, req);
                return new OkObjectResult(new EventDto(newevent));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }


        }
    }
}
