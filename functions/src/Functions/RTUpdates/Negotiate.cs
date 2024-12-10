using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Functions
{
    public static class Negotiate
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RTUpdates/Negotiate")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "yourHub")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}