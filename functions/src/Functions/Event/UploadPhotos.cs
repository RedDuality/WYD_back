

using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class UploadPhotos(ILogger<UploadPhotos> logger, AuthorizationService authorizationService, RequestService requestService, EventService eventService)
    {
        private readonly ILogger<UploadPhotos> _logger = logger;
        private readonly RequestService _requestService = requestService;
        private readonly EventService _eventService = eventService;
        private readonly AuthorizationService _authorizationService = authorizationService;


        [Function("UploadPhotos")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Upload/Photos/{eventHash}")] HttpRequest req, String eventHash, FunctionContext executionContext)
        {

            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.UPDATE_EVENT);

                HashSet<BlobData> newBlobs = await _requestService.DeserializeRequestBodyAsync<HashSet<BlobData>>(req);
            
                var newevent = await _eventService.AddMultipleBlobs(eventHash, newBlobs);

                await _requestService.NotifyAsync(newevent, UpdateType.UpdatePhotos, currentProfile);

                return new OkObjectResult(new EventDto(newevent));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
