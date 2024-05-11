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
    public class DeleteUser
    {
        private readonly ILogger<DeleteUser> _logger;

        private readonly UserController _userController;

        public DeleteUser(ILogger<DeleteUser> logger)
        {
            _logger = logger;
            _userController = new UserController();
        }

        [Function("DeleteUser")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "User/{userId}")] HttpRequest req, string userId, FunctionContext executionContext)
        {

            int id;
            try
            {
                id = Int32.Parse(userId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }

            var response = _userController.Delete(id);

            return new OkObjectResult(response);
        }
    }
}
