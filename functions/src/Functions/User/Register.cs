

using System.Text.Json;
using Controller;
using dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;


namespace Functions
{
    public class Register
    {
        private readonly ILogger<Register> _logger;
        private readonly AuthService _authController;

        public Register(ILogger<Register> logger, AuthService authService)
        {
            _logger = logger;
            _authController = authService;
        }

        [Function("Register")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Register")] HttpRequest req, FunctionContext executionContext)
        {

            //LoginDto ciao = new LoginDto{UserName = "antonio", Password= "password"};


            //_logger.LogInformation(JsonConvert.SerializeObject(ciao));
            //_logger.LogInformation(JsonConvert.SerializeObject(registerDto));

            LoginDto? registerData = await JsonSerializer.DeserializeAsync<LoginDto>(req.Body);
            if (registerData != null)
            {
                try
                {
                    var user = _authController.Register(registerData);
                    return new OkObjectResult(user);
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
