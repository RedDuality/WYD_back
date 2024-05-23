

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
    public class ShareEvent
    {
        private readonly ILogger<ShareEvent> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public ShareEvent(ILogger<ShareEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
            _authController = new AuthController();
        }

        [Function("ShareEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Share/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
        {
            User? user = _authController.VerifyRequest(req);
            if(user == null)
                return new ForbidResult();

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
                return new OkObjectResult(newevent);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
