using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class Retrieve
    {
        private readonly ILogger<Retrieve> _logger;

        private readonly AuthService _authController;

        public Retrieve(ILogger<Retrieve> logger, AuthService authService)
        {
            _logger = logger;
            _authController = authService;
        }

        [Function("Retrieve")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Retrieve")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            return new OkObjectResult(new UserDto(user));

        }
    }
}
