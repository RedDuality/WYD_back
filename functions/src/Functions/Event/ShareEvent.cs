

using System.Text;
using System.Text.Json;
using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;


namespace Functions
{
    public class ShareEvent
    {
        private readonly ILogger<ShareEvent> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;
        private readonly ProfileService _profileService;

        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public ShareEvent(ILogger<ShareEvent> logger, EventService eventService, AuthService authService, JsonSerializerOptions jsonSerializerOptions, ProfileService profileService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
            _profileService = profileService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("ShareEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Share/Community/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            User user;
            try{
                user = await _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            int id;
            try
            {
                id = Int32.Parse(eventId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            
            var profileIdList = JsonSerializer.Deserialize<List<int>>(requestBody, _jsonSerializerOptions); 

            if (profileIdList != null)
            {
                 List<Profile> profiles = _profileService.GetProfiles(profileIdList);
                var newevent = _eventController.Share(id, profiles);
                return new OkObjectResult(newevent);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
