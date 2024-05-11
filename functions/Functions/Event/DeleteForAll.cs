

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
    public class DeleteEventForAll
    {
        private readonly ILogger<DeleteEventForAll> _logger;
        private readonly EventController _eventController;

        public DeleteEventForAll(ILogger<DeleteEventForAll> logger)
        {
            _logger = logger;
            _eventController = new EventController();
        }

        [Function("DeleteEventForAll")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/ForAll/{eventId}")] HttpRequestData req, string eventId, FunctionContext executionContext)
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

            var result = _eventController.Delete(id);
            return new OkObjectResult(result);

        }
    }
}
