

using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace Functions
{
    public class VerifyToken
    {
        private readonly ILogger<VerifyToken> _logger;
        private readonly AuthService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public VerifyToken(ILogger<VerifyToken> logger, AuthService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("VerifyToken")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Auth/VerifyToken")] HttpRequest req)
        {
            TokenRequest? VerifyToken = await JsonSerializer.DeserializeAsync<TokenRequest>(req.Body, _jsonSerializerOptions);
            if (VerifyToken != null && VerifyToken.Token != null)
            {
                try
                {
                    string uid = await _authController.CheckFirebaseTokenAsync(VerifyToken.Token);
                    return new OkObjectResult(uid);
                } catch (Exception e) { return new BadRequestObjectResult(e.Message); }

            }
            else return new BadRequestObjectResult("Bad Json Formatting");

        }
    }

    // TokenRequest class to receive the token from the client
    public class TokenRequest
    {
        public string? Token { get; set; }
    }
}
