
using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class SearchByTag
    {
        private readonly ILogger<SearchByTag> _logger;

        private readonly ProfileService _profileService;

        public SearchByTag(ILogger<SearchByTag> logger, ProfileService profileService)
        {
            _logger = logger;
            _profileService = profileService;
        }

        [Function("SearchByTag")]
        public IActionResult RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Profile/SearchByTag/{searchTag}")] HttpRequest req, string searchTag)
        {

            try
            {
                var profiles = _profileService.SearchByTag(searchTag);
                return new OkObjectResult(profiles);
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
