

using System.Text;
using System.Text.Json;
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class CreateEvent
    {
        private readonly ILogger<CreateEvent> _logger;
        private readonly RequestService _requestService;
        private readonly EventService _eventService;
        private readonly AuthorizationService _authorizationService;

        public CreateEvent(ILogger<CreateEvent> logger, AuthorizationService authorizationService,RequestService requestService, EventService eventService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _requestService = requestService;
            _eventService = eventService;
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Create")] HttpRequest req, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CONFIRM_EVENT);

                var myevent = await _requestService.DeserializeRequestBodyAsync<EventDto>(req);
                var newevent = await _eventService.Create(myevent, currentProfile);
                
                return new OkObjectResult(new EventDto(newevent));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }


        }
    }
}
