

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
    public class CreateEvent
    {
        private readonly ILogger<CreateEvent> _logger;
        private readonly EventController _eventController;

        public CreateEvent(ILogger<CreateEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req, FunctionContext executionContext)
        {

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            
            var myevent = JsonConvert.DeserializeObject<Event>(requestBody);
            if (myevent != null)
            {
                User uc = new UserController().Get(1);
                var newevent = _eventController.Create(myevent, uc);
                string result = JsonConvert.SerializeObject(newevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
