

using System.Text.Json;
using Controller;
using dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class Login
    {
        private readonly ILogger<Login> _logger;
        private readonly AuthService _authController;

        public Login(ILogger<Login> logger, AuthService authService)
        {
            _logger = logger;
            _authController = authService;
        }

        [Function("Login")]
        public async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Auth/Login")] HttpRequest req, FunctionContext executionContext)
        {

            LoginDto? loginDto = await JsonSerializer.DeserializeAsync<LoginDto>(req.Body);
            if (loginDto != null)
            {
                try
                {
                    
                    if(_authController.Login(loginDto, out string token))
                        return new OkObjectResult(token);
                    else
                        return new BadRequestObjectResult("Bad Credentials");
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(e.Message);
                }

            }
            else return new BadRequestObjectResult("Bad Json Formatting");


        }
    }
}
