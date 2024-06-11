using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;
using System.Text.Json;

namespace Functions
{
    public class RetrieveCommunities
    {
        private readonly ILogger<RetrieveCommunities> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public RetrieveCommunities(ILogger<RetrieveCommunities> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("RetrieveCommunities")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Communities")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            var communities = user.Communities;
            string result = JsonSerializer.Serialize(communities);

            return new OkObjectResult(result);

        }
    }
}
