using Controller;
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
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public ListEvents(ILogger<ListEvents> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("ListEvents")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/Events")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try{
                user = await _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 
            
            //var eventi = user.Events;

            //return new OkObjectResult(eventi);
             return new OkObjectResult(null);
        }
    }
}
