using System.Text;
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class AddPhoto(ILogger<CreateCommunity> logger, RequestService requestService, AuthorizationService authorizationService, EventService eventService)
    {
        private readonly ILogger<CreateCommunity> _logger = logger;
        private readonly AuthorizationService _authorizationService = authorizationService;
        private readonly RequestService _requestService = requestService;
        private readonly EventService _eventService = eventService;

        [Function("AddPhoto")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Photo/Add/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.ADD_PHOTO);

                BlobData blobData = await _requestService.DeserializeRequestBodyAsync<BlobData>(req);
                await _eventService.AddBlobAsync(int.Parse(eventId), blobData);

                return new OkObjectResult("Blob saved with success");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
