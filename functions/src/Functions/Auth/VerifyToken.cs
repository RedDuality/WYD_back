

using System.Text.Json;
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class VerifyToken
    {
        private readonly ILogger<VerifyToken> _logger;
        private readonly AuthenticationService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public VerifyToken(ILogger<VerifyToken> logger, AuthenticationService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("VerifyToken")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Auth/VerifyToken")] HttpRequest req)
        {
            TokenRequest? TR = await JsonSerializer.DeserializeAsync<TokenRequest>(req.Body, _jsonSerializerOptions);
            if (TR != null && TR.Token != null)
            {
                try
                {
                    User user = await _authController.VerifyTokenAsync(TR.Token);
                    return new OkObjectResult(new RetrieveUserDto(user));
                }
                catch (Exception e) { return new BadRequestObjectResult(e.Message); }

            }
            else return new BadRequestObjectResult("Bad Json Formatting");

        }
    }

}
