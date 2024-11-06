

using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class RetrieveFromHash
    {
        private readonly ILogger<RetrieveFromHash> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public RetrieveFromHash(ILogger<RetrieveFromHash> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("RetrieveFromHash")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Event/Retrieve/{eventHash}")] HttpRequest req, string eventHash, FunctionContext executionContext)
        {
            User user;
            try{
                user = _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 



            Event? e = _eventController.RetrieveFromHash(eventHash);

            return e == null ? new NotFoundObjectResult("") : new OkObjectResult(e);

        }
    }
}
