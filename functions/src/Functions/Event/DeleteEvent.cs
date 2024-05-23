

using System.Text;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Model;
using Newtonsoft.Json;



namespace Functions
{
    public class DeleteEvent
    {
        private readonly ILogger<DeleteEvent> _logger;
        private readonly EventController _eventController;

        private readonly AuthController _authController;

        public DeleteEvent(ILogger<DeleteEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
            _authController = new AuthController();
        }

        [Function("DeleteEvent")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            int id;
            try
            {
                id = Int32.Parse(eventId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }

            User? user = _authController.VerifyRequest(req);
            if(user == null)
                return new ForbidResult();

            var result = _eventController.DeleteForUser(id, user.Id);
            return new OkObjectResult(result);

        }
    }
}
