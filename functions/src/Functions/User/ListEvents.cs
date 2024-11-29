using Controller;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class ListEvents
    {
        private readonly ILogger<ListEvents> _logger;
        private readonly AuthService _authController;
        private readonly UserService _userService;

        public ListEvents(ILogger<ListEvents> logger, AuthService authService, UserService userService)
        {
            _logger = logger;

            _authController = authService;

            _userService = userService;
        }

        [Function("ListEvents")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Events")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authController.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }
            
            List<EventDto> eventi = await _userService.RetrieveEventsAsync(user);
            return new OkObjectResult(eventi);
        }
    }
}
