

using System.Text;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Model;
using Newtonsoft.Json;



namespace Functions.Community
{
    public class UpdateCommunity
    {
        private readonly ILogger<UpdateCommunity> _logger;
        private readonly CommunityController _communityController;
        private readonly AuthController _authController;

        public UpdateCommunity(ILogger<UpdateCommunity> logger, CommunityController communityController, AuthController authController)
        {
            _logger = logger;
            _communityController = communityController;
            _authController = authController;
        }

        [Function("UpdateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Community/Update/{communityId}")] HttpRequest req, string communityId, FunctionContext executionContext)
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
            var newCommunity = JsonConvert.DeserializeObject<Model.Community>(requestBody);

            if (newCommunity != null)
            {
                var updated = _communityController.Update(user, id, newCommunity);
                return new OkObjectResult(updated);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
