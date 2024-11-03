using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace Functions
{
    public class VerifyTokenAndCreate
    {
        private readonly ILogger<VerifyTokenAndCreate> _logger;
        private readonly AuthService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public VerifyTokenAndCreate(ILogger<VerifyTokenAndCreate> logger, AuthService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("VerifyTokenAndCreate")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Auth/VerifyTokenAndCreate")] HttpRequest req)
        {
            TokenRequest? TR = await JsonSerializer.DeserializeAsync<TokenRequest>(req.Body, _jsonSerializerOptions);
            if (TR != null && TR.Token != null)
            {
                try
                {
                    await _authController. VerifyTokenAndCreateAsync(TR.Token);
                    return new OkObjectResult("");
                } catch (Exception e) { return new BadRequestObjectResult(e.Message); }

            }
            else return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
