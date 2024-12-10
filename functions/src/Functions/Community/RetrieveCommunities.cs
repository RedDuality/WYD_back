


using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class RetrieveCommunities(ILogger<RetrieveCommunities> logger, AuthorizationService authorizationService)
    {
        private readonly ILogger<RetrieveCommunities> _logger = logger;
        private readonly AuthorizationService _authorizationService = authorizationService;


        [Function("RetrieveCommunities")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Community/Retrieve/Profile/{ProfileId}")] HttpRequest req, string ProfileId, FunctionContext executionContext)
        {
            try
            {
                Profile currentProfile = await _authorizationService.VerifyRequest(req, UserPermissionOnProfile.READ_COMMUNITY);

                return new OkObjectResult(currentProfile.Communities.Select(c => new CommunityDto(c, currentProfile)).ToList());
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
