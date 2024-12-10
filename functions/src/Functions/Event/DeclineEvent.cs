

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
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Decline/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
    {
        try
        {
            Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.CONFIRM_EVENT);

            _eventService.ConfirmOrDecline(int.Parse(eventId), currentProfile, false);
            return new OkObjectResult("");
        }
        catch (Exception ex)
        {
            return RequestService.GetErrorResult(ex);
        }

    }
}

