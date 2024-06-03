

using System.Text;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class DeleteEventForAll
    {
        private readonly ILogger<DeleteEventForAll> _logger;
        private readonly EventService _eventController;
        private readonly AuthService _authController;

        public DeleteEventForAll(ILogger<DeleteEventForAll> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
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
            User user;
            try{
                user = _authController.VerifyRequest(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            return _eventController.Delete(id, user.Id) ? new OkObjectResult("") : new BadRequestResult();

        }
    }
}
