using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace Functions.Test
{
    public class Ping
    {
        private readonly ILogger<Ping> _logger;


        public Ping(ILogger<Ping> logger)
        {
            _logger = logger;
        }

        [Function("Ping")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Ping")] HttpRequest req, FunctionContext executionContext)
        {
            return new OkObjectResult("I'm Here");
        }
    }
}
 