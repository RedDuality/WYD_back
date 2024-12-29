

using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions;

public class DeclineEvent(ILogger<DeclineEvent> logger, EventService eventService, AuthorizationService authorizationService, RequestService requestService)
{
    private readonly ILogger<DeclineEvent> _logger = logger;
    private readonly EventService _eventService = eventService;
    private readonly AuthorizationService _authorizationService = authorizationService;
    private readonly RequestService requestService = requestService;

    [Function("DeclineEvent")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Decline/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
    {
        try
        {
            Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CONFIRM_EVENT);

            Event ev = _eventService.ConfirmOrDecline(eventHash, currentProfile, false);

            await requestService.NotifyAsync(ev, UpdateType.DeclineEvent, currentProfile);
            return new OkObjectResult("");
        }
        catch (Exception ex)
        {
            return RequestService.GetErrorResult(ex);
        }

    }
}

