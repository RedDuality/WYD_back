using System.Text.Json;
using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class RetrieveById
    {
        private readonly ILogger<RetrieveById> _logger;

        private readonly UserService _userController;

        public RetrieveById(ILogger<RetrieveById> logger, UserService userService)
        {
            _logger = logger;
            _userController = userService;
        }

        [Function("RetrieveById")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/RetrieveById/{id}")] HttpRequest req, string id)
        {
            int userId;
            try
            {
                userId = Int32.Parse(id);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }


            User? user = _userController.RetrieveOrNull(userId);
            if (user != null)
                return new OkObjectResult(user);
            return new NotFoundObjectResult("User not found");

        }
    }
}
