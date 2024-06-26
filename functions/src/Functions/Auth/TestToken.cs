
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class TestToken
    {
        private readonly ILogger<TestToken> _logger;
        private readonly AuthService _authController;

        public TestToken(ILogger<TestToken> logger, AuthService authService)
        {
            _logger = logger;
            _authController = authService;
        }

        [Function("TestToken")]
        public ActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Auth/TestToken")] HttpRequest req)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            return new OkObjectResult("Token is valid");


        }
    }
}
