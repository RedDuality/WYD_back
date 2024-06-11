
using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions.Community
{
    public class CreateCommunity
    {
        private readonly ILogger<CreateCommunity> _logger;
        private readonly CommunityService _communityController;
        private readonly AuthService _authController;

        public CreateCommunity(ILogger<CreateCommunity> logger, CommunityService communityService, AuthService authService)
        {
            _logger = logger;
            _communityController = communityService;
            _authController = authService;
        }

        [Function("CreateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Community/Create/{name}")] HttpRequest req, string name, FunctionContext executionContext)
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
            var userIdList = JsonSerializer.Deserialize<List<int>>(requestBody);

            if (userIdList != null)
            {
                var newcommunity = _communityController.Create(user, name, userIdList);
                return new OkObjectResult(newcommunity);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
