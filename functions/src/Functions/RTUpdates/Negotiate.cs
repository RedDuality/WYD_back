using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Service;
using Model;
/*
namespace Functions
{
    public class Negotiate(UpdatesHub updatesHub, RequestService requestService)
    {
        private readonly UpdatesHub _updatesHub = updatesHub;

        private readonly RequestService _requestService = requestService;

        //[Function("Negotiate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RTUpdates/negotiate/v1")] HttpRequest req,
            FunctionContext executionContext)
        {
            try
            {
                User user = await _requestService.VerifyRequestAsync(req);
                var connectionInfo = await _updatesHub.Negotiate(user.Hash);
                return new JsonResult(connectionInfo);
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}*/