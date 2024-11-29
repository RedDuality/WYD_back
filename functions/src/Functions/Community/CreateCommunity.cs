

using System.Text;
using System.Text.Json;
using Controller;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;



namespace Functions
{
    public class CreateCommunity
    {
        private readonly ILogger<CreateCommunity> _logger;
        private readonly CommunityService _communityService;
        private readonly AuthService _authService;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CreateCommunity(ILogger<CreateCommunity> logger, AuthService authService, CommunityService communityService, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _authService = authService;
            _communityService = communityService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("CreateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Community/Create")] HttpRequest req, FunctionContext executionContext)
        {
            User user;
            try
            {
                user = await _authService.VerifyRequestAsync(req);
            }
            catch (Exception) { return new StatusCodeResult(StatusCodes.Status403Forbidden); }

            string requestBody;
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var communityDto = JsonSerializer.Deserialize<CreateCommunityDto>(requestBody, _jsonSerializerOptions);


            if (communityDto != null)
            {
                communityDto.Users.Add(new UserDto(user));
                var newCommunity = _communityService.Create(communityDto);
                return new OkObjectResult(newCommunity);
            }
            return new BadRequestObjectResult("Bad Json Formatting");

        }
    }
}
