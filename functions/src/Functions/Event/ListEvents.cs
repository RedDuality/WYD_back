using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Model;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Functions
{
    public class ListEvents
    {
        private readonly ILogger<ListEvents> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public ListEvents(ILogger<ListEvents> logger)
        {
            _logger = logger;
            _eventController = new EventController();
            _authController = new AuthController();
        }

        [Function("ListEvents")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListEvents")] HttpRequest req, FunctionContext executionContext)
        {

            User? user = _authController.VerifyRequest(req);
            if(user == null)
                return new ForbidResult();

            var eventi = _eventController.GetEvents(user.Id);
            string result = JsonSerializer.Serialize(eventi);

            return new OkObjectResult(result);
            
        }
    }
}
