

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
    public class CreateEvent
    {
        private readonly ILogger<CreateEvent> _logger;
        private readonly EventController _eventController;
        private readonly AuthController _authController;

        public CreateEvent(ILogger<CreateEvent> logger)
        {
            _logger = logger;
            _authController = new AuthController();
            _eventController = new EventController();
        }

        [Function("CreateEvent")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, FunctionContext executionContext)
        {

            User? user = _authController.VerifyRequest(req);
            if(user == null)
                return new ForbidResult();

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myevent = JsonConvert.DeserializeObject<Event>(requestBody);


            if (myevent != null)
            {
                User uc = new UserController().Get(user.Id);
                var newevent = _eventController.Create(myevent, uc);
                string result = JsonConvert.SerializeObject(newevent);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
