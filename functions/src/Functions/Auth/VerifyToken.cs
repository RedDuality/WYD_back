
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class VerifyToken(ILogger<VerifyToken> logger, RequestService requestService)
    {
        private readonly ILogger<VerifyToken> _logger = logger;

        private readonly RequestService _requestService = requestService;

        [Function("VerifyToken")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Auth/VerifyToken")] HttpRequest req)
        {
            try
            {
                User user = await _requestService.VerifyRequestAsync(req);
                return new OkObjectResult(new RetrieveUserDto(user));
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }

}
