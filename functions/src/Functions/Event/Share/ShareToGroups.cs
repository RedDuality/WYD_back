

using System.Text;
using System.Text.Json;
using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;
using Microsoft.IdentityModel.Tokens;
using Dto;


namespace Functions
{
    public class ShareToGroups
    {
        private readonly ILogger<ShareToGroups> _logger;
        private readonly EventService _eventController;
        private readonly AuthenticationService _authController;
        private readonly ProfileService _profileService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public ShareToGroups(ILogger<ShareToGroups> logger, EventService eventService, AuthenticationService authService, JsonSerializerOptions jsonSerializerOptions, ProfileService profileService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
            _profileService = profileService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("ShareToGroups")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Share/Groups/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authController.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

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
            using (StreamReader reader = new(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var groupIds = JsonSerializer.Deserialize<HashSet<int>>(requestBody, _jsonSerializerOptions);
            if (groupIds.IsNullOrEmpty()) return new BadRequestObjectResult("Bad Json Formatting");


            var newevent = _eventController.ShareToGroups(id, groupIds!);
            return new OkObjectResult(new EventDto(newevent));



        }
    }
}
