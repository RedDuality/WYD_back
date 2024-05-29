
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
    public class CreateCommunity
    {
        private readonly ILogger<CreateCommunity> _logger;
        private readonly CommunityController _communityController;
        private readonly AuthController _authController;

        public CreateCommunity(ILogger<CreateCommunity> logger, CommunityController communityController, AuthController authController)
        {
            _logger = logger;
            _communityController = communityController;
            _authController = authController;
        }

        [Function("CreateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Community/Create/{name}")] HttpRequest req, string name, FunctionContext executionContext)
        {
            User user;
            try{
                user = _authController.VerifyRequest(req);
            }catch(Exception){return new StatusCodeResult(StatusCodes.Status403Forbidden);} 


            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var userIdList = JsonConvert.DeserializeObject<List<int>>(requestBody);

            if (userIdList != null)
            {
                var newcommunity = _communityController.Create(user, name, userIdList);
                return new OkObjectResult(newcommunity);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
