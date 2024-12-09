

using System.Text;
using Service;
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
        private readonly AuthenticationService _authController;

        public DeleteEventForAll(ILogger<DeleteEventForAll> logger, EventService eventService, AuthenticationService authService)
        {
            _logger = logger;
            _eventController = eventService;
            _authController = authService;
        }

        [Function("DeleteEventForAll")]
        public async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Event/DeleteForAll/{eventId}")] HttpRequest req, string eventId, FunctionContext executionContext)
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
                user = await _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            //return _eventController.Delete(id, user.Id) ? new OkObjectResult("") : new BadRequestResult();
            return null;
        }
    }
}
