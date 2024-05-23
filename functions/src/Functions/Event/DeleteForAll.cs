

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
    public class DeleteEventForAll
    {
        private readonly ILogger<DeleteEventForAll> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public DeleteEventForAll(ILogger<DeleteEventForAll> logger)
        {
            _logger = logger;
            _eventController = new EventController();
            _authController = new AuthController();
        }

        [Function("DeleteEventForAll")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/ForAll/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            int id;
            try
            {
                id = int.Parse(eventId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }

            User? user = _authController.VerifyRequest(req);
            if (user == null)
                return new ForbidResult();

            return _eventController.Delete(id, user.Id) ? new OkObjectResult("") : new BadRequestResult();

        }
    }
}
