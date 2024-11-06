

using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class UpdateUser
    {
        private readonly ILogger<UpdateUser> _logger;
        private readonly UserService _userController;
        private readonly AuthService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public UpdateUser(ILogger<UpdateUser> logger, AuthService authService, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _userController = userService;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("UpdateUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User/Update")] HttpRequest req, FunctionContext executionContext)
        {

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var myuser = JsonSerializer.Deserialize<User>(requestBody, _jsonSerializerOptions);
            
            User user;
            try{
                user = await _authController.VerifyRequestAsync(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 

            if (myuser != null)
            {
                var result = _userController.Update(user, myuser);
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
