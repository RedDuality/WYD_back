

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
    public class UpdateEvent
    {
        private readonly ILogger<UpdateEvent> _logger;
        private readonly EventService _eventService;
        private readonly AuthService _authService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public UpdateEvent(ILogger<UpdateEvent> logger, EventService eventService, AuthService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _eventService = eventService;
            _authService = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("UpdateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Update")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authService.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonSerializer.Deserialize<EventDto>(requestBody, _jsonSerializerOptions);

            if (myevent != null)
            {
                try
                {
                    _eventService.UpdateField(myevent);
                    return new OkObjectResult("");
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
