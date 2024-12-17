using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Service;

namespace Functions
{
    public class SendUpdateNotification(NotificationService rtservice, RequestService requestService)
    {
        private readonly NotificationService rTService = rtservice;

        private readonly RequestService _requestService = requestService;

        [Function("SendUpdateNotification")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SendUpdateNotification")] HttpRequest req,
            FunctionContext executionContext)
        {
            try
            {
                await rTService.SendUpdateNotifications(["NDjmfkkUXxzINlM47MycQ"], UdpateType.Event, 2);
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}