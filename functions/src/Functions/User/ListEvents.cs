using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class ListEvents
    {
        private readonly ILogger<ListEvents> _logger;
        private readonly AuthorizationService _authorizationService;
        private readonly UserService _userService;

        public ListEvents(ILogger<ListEvents> logger, AuthorizationService authorizationService, UserService userService)
        {
            _logger = logger;

            _authorizationService = authorizationService;

            _userService = userService;
        }

        [Function("ListEvents")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Events")] HttpRequest req, FunctionContext executionContext)
        {
            try
            {
                User user = await _authorizationService.VerifyRequest(req);

                List<EventDto> eventi = await UserService.RetrieveEventsAsync(user);

                return new OkObjectResult(eventi);
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
