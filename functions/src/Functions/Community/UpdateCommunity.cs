

using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Model;


namespace Functions.Community
{
    public class UpdateCommunity
    {
        private readonly ILogger<UpdateCommunity> _logger;
        private readonly CommunityService _communityController;
        private readonly AuthService _authController;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public UpdateCommunity(ILogger<UpdateCommunity> logger, CommunityService communityService, AuthService authService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _communityController = communityService;
            _authController = authService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("UpdateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Community/Update/{communityId}")] HttpRequest req, string communityId, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = _authController.VerifyRequest(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            int id;
            try
            {
                id = Int32.Parse(communityId);
            }
            catch (FormatException)
            {
                return new BadRequestObjectResult("Id Format wrong");
            }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var newCommunity = JsonSerializer.Deserialize<Model.Community>(requestBody, _jsonSerializerOptions);

            if (newCommunity != null)
            {
                var updated = _communityController.Update(user, id, newCommunity);
                return new OkObjectResult(updated);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
