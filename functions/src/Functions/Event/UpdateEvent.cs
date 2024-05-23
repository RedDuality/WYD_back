

using System.Text;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;
using Newtonsoft.Json;



namespace Functions
{
    public class UpdateEvent
    {
        private readonly ILogger<UpdateEvent> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public UpdateEvent(ILogger<UpdateEvent> logger)
        {
            _logger = logger;
            _eventController = new EventController();
            _authController = new AuthController();
        }

        [Function("UpdateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Event")] HttpRequest req, FunctionContext executionContext)
        {

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonConvert.DeserializeObject<Event>(requestBody);
            
            User? user = _authController.VerifyRequest(req);
            if (user == null)
                return new ForbidResult();

            if (myevent != null)
            {
                var result = _eventController.Update(myevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
