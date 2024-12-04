
using Service;
using Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class MakeMultiGroup(ILogger<MakeMultiGroup> logger, RequestService requestService, AuthorizationService authorizationService, CommunityService communityService, ProfileService profileService)
    {
        private readonly ILogger<MakeMultiGroup> _logger = logger;
        private readonly CommunityService _communityService = communityService;
        private readonly AuthorizationService _authorizationService = authorizationService;
        private readonly RequestService _requestService = requestService;
        private readonly ProfileService _ProfileService = profileService;


        [Function("MakeMultiGroup")]
        public  IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Community/MultiGroup/{CommunityId}")] HttpRequest req, string CommunityId, FunctionContext executionContext)
        {
            try
            {
                Community c = _communityService.Retrieve(int.Parse(CommunityId));
                _communityService.MakeMultiGroup(c);


                return new OkObjectResult("");

            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }
        }
    }
}
