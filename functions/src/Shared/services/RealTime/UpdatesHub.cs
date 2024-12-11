
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;
using Model;
using Service;

namespace Functions
{
    [SignalRConnection("AzureSignalRConnectionString")]
    public class UpdatesHub(IServiceProvider serviceProvider, ILogger<UpdatesHub> logger, RequestService requestService) : ServerlessHub<IChatClient>(serviceProvider)
    {
        private const string HubName = nameof(Functions);
        private readonly ILogger _logger = logger;
        
        private readonly RequestService _requestService = requestService;
/*
        public async Task<BinaryData> Negotiate( string userHash)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return await NegotiateAsync(new() { UserId = userHash });
            //return negotiateResponse.ToArray();
        }
*/
        [Function("negotiate")]
        public async Task<HttpResponseData> Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            User user = await _requestService.VerifyRequestAsync(req);

            var negotiateResponse = await NegotiateAsync(new() { UserId = user.Hash});
            var response = req.CreateResponse();
            await response.WriteBytesAsync(negotiateResponse.ToArray());
            return response;
        }

        public Task JoinGroup([SignalRTrigger(HubName, "messages", "JoinGroup", "connectionId", "groupName")] SignalRInvocationContext invocationContext, string connectionId, string groupName)
        {
            return Groups.AddToGroupAsync(connectionId, groupName);
        }


        public Task LeaveGroup([SignalRTrigger(HubName, "messages", "LeaveGroup", "connectionId", "groupName")] SignalRInvocationContext invocationContext, string connectionId, string groupName)
        {
            return Groups.RemoveFromGroupAsync(connectionId, groupName);
        }

        public Task JoinUserToGroup([SignalRTrigger(HubName, "messages", "JoinUserToGroup", "userName", "groupName")] SignalRInvocationContext invocationContext, string userName, string groupName)
        {
            return UserGroups.AddToGroupAsync(userName, groupName);
        }

        public Task LeaveUserFromGroup([SignalRTrigger(HubName, "messages", "LeaveUserFromGroup", "userName", "groupName")] SignalRInvocationContext invocationContext, string userName, string groupName)
        {
            return UserGroups.RemoveFromGroupAsync(userName, groupName);
        }

        public void OnDisconnected([SignalRTrigger(HubName, "connections", "disconnected")] SignalRInvocationContext invocationContext)
        {
            _logger.LogInformation($"{invocationContext.ConnectionId} has disconnected");
        }
    }

    public interface IChatClient
    {

    }


}