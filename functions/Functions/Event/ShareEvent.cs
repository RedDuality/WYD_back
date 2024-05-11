

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
    public class ShareEvent
    {
        private readonly ILogger<ShareEvent> _logger;
        private readonly EventController _eventController;

        public ShareEvent(ILogger<ShareEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
        }

        [Function("ShareEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Share/{eventId}")] HttpRequestData req, string eventId, FunctionContext executionContext)
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

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var userIdList = JsonConvert.DeserializeObject<List<int>>(requestBody);

            if (userIdList != null)
            {
                
                var newevent = _eventController.Share(id, userIdList);
                string result = JsonConvert.SerializeObject(newevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
