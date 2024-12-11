
using Service;
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
        private readonly RequestService _requestService;

        public TestToken(ILogger<TestToken> logger, RequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;
        }

        [Function("TestToken")]
        public async Task<ActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Auth/TestToken")] HttpRequest req)
        {
            User user;
            try
            {
                user =  await _requestService.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            return new OkObjectResult("Token is valid");


        }
    }
}
