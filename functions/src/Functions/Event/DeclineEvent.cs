

using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions;

public class DeclineEvent(ILogger<DeclineEvent> logger, EventService eventService, AuthorizationService authorizationService)
{
    private readonly ILogger<DeclineEvent> _logger = logger;
    private readonly EventService _eventService = eventService;
    private readonly AuthorizationService _authorizationService = authorizationService;

    [Function("DeclineEvent")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Decline/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
    {
        try
        {
            Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CONFIRM_EVENT);

            _eventService.ConfirmOrDecline(eventHash, currentProfile, false);
            return new OkObjectResult("");
        }
        catch (Exception ex)
        {
            return RequestService.GetErrorResult(ex);
        }

    }
}

