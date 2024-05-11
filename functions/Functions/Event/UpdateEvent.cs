

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
    public class UpdateEvent
    {
        private readonly ILogger<UpdateEvent> _logger;
        private readonly EventController _eventController;

        public UpdateEvent(ILogger<UpdateEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
        }

        [Function("UpdateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event")] HttpRequestData req, FunctionContext executionContext)
        {

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            
            var myevent = JsonConvert.DeserializeObject<Event>(requestBody);
            if (myevent != null)
            {
                var result = _eventController.Update(myevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
