

using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;


namespace Functions
{
    public class ConfirmFromHash
    {
        private readonly ILogger<ConfirmFromHash> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public ConfirmFromHash(ILogger<ConfirmFromHash> logger, EventService eventService, AuthService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("ConfirmFromHash")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event/Confirm/Hash/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }


            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var confirmed = JsonSerializer.Deserialize<bool>(requestBody, _jsonSerializerOptions);


            try
            {
                var result = _eventController.ConfirmFromHash(user, eventHash, confirmed);
                return new OkObjectResult("");
            }
            catch (Exception e) { return new BadRequestObjectResult(e.ToString()); }


        }
    }
}
