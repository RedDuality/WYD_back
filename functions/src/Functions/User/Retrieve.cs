using Service;
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

        private readonly RequestService _requestService;

        public Retrieve(ILogger<Retrieve> logger, RequestService requestService)
        {
            _logger = logger;
            _requestService = requestService;
        }

        [Function("Retrieve")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Retrieve")] HttpRequest req, FunctionContext executionContext)
        {
            try
            {
                User user = await _requestService.VerifyRequestAsync(req);
                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
