
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class CreateCommunity(ILogger<CreateCommunity> logger, RequestService requestService, AuthorizationService authorizationService, CommunityService communityService, ProfileService profileService)
    {
        private readonly ILogger<CreateCommunity> _logger = logger;
        private readonly CommunityService _communityService = communityService;
        private readonly AuthorizationService _authorizationService = authorizationService;
        private readonly RequestService _requestService = requestService;
        private readonly ProfileService _ProfileService = profileService;


        [Function("CreateCommunity")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Community/Create/{ProfileId}")] HttpRequest req, string ProfileId, FunctionContext executionContext)
        {
            try
            {

                Profile currentProfile = await _authorizationService.VerifyRequest(req, int.Parse(ProfileId), "CREATE_COMMUNITY");

                var createCommunityDto = await _requestService.DeserializeRequestBodyAsync<CreateCommunityDto>(req);
            
                var community = _communityService.Create(createCommunityDto, currentProfile);

                return new OkObjectResult(new CommunityDto(community, currentProfile));

            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
