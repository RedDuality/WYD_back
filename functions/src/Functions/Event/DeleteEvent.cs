

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
    public class DeleteEvent
    {
        private readonly ILogger<DeleteEvent> _logger;
        private readonly EventService _eventController;

        private readonly AuthService _authController;

        public DeleteEvent(ILogger<DeleteEvent> logger, EventService eventService, AuthService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("DeleteEvent")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/Delete/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
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
            User user;
            try{
                user = _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            //var result = _eventController.DeleteForUser(id, user.Id);
            return new OkObjectResult(true);

        }
    }
}
