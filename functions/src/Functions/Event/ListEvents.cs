using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;
using System.Text.Json;

namespace Functions
{
    public class ListEvents
    {
        private readonly ILogger<ListEvents> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public ListEvents(ILogger<ListEvents> logger, EventController eventController, AuthController authController)
        {
            _logger = logger;
            _eventController = eventController;
            _authController = authController;
        }

        [Function("ListEvents")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListEvents")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try{
                user = _authController.VerifyRequest(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 
            
                

            var eventi = user.Events;
            string result = JsonSerializer.Serialize(eventi);

            return new OkObjectResult(result);
            
        }
    }
}
