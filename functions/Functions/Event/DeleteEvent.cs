

using System.Text;
using Controller;
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

        public DeleteEvent(ILogger<DeleteEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
        }

        [Function("DeleteEvent")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/{eventId}")] HttpRequestData req, string eventId, FunctionContext executionContext)
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

            var result = _eventController.DeleteForUser(id, 1);
            return new OkObjectResult(result);

        }
    }
}
