using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
namespace Functions
{
    public static class AddToProfileGroup
    {
        [FunctionName("addToProfileGroup")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "RTUpdates/AddToProfileGroup")] HttpRequest req,
            [SignalR(HubName = "yourHub")] IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            string userId = req.Query["userId"];
            string profileId = req.Query["profileId"];
            string groupName = profileId; // Each profile has its own group

            await signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    UserId = userId,
                    GroupName = groupName,
                    Action = GroupAction.Add
                });

            return new OkResult();
        }
    }
}