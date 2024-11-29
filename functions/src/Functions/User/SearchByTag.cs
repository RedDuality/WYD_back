using System.Text;
using System.Text.Json;
using Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Model;

namespace Functions
{
    public class SearchByTag
    {
        private readonly ILogger<SearchByTag> _logger;

        private readonly UserService _userService;

        
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public SearchByTag(ILogger<SearchByTag> logger, UserService userService, JsonSerializerOptions jsonSerializerOptions)
        {
            _logger = logger;
            _userService = userService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [Function("SearchByTag")]
        public IActionResult RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/SearchByTag/{searchTag}")] HttpRequest req, string searchTag)
        {

            if (searchTag != null)
            {
                try
                {
                    var users = _userService.SearchByTag(searchTag);
                    return new OkObjectResult(users);
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }
            return new BadRequestObjectResult("Bad searchTag Formatting");

        }
    }
}
