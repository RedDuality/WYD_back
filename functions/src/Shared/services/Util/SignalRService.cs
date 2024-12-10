using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Functions.Services
{
    public class SignalRService
    {
        private readonly IAsyncCollector<SignalRMessage> _signalRMessages;

        public SignalRService(IAsyncCollector<SignalRMessage> signalRMessages)
        {
            _signalRMessages = signalRMessages;
        }

        public async Task SendMessageToProfile(string profileId, string message)
        {
            await _signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ReceiveMessage",
                    GroupName = profileId, // Send to the profile's group
                    Arguments = new[] { message }
                });
        }
    }
}