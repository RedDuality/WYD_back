
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace Functions.Test
{
    public class PingDB
    {
        private readonly ILogger<PingDB> _logger;

        private readonly WydDbContext db;

        public PingDB(ILogger<PingDB> logger, WydDbContext dbContext)
        {
            _logger = logger;
            db = dbContext;
        }

        [Function("PingDB")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PingDB")] HttpRequest req, FunctionContext executionContext)
        {
            return new OkObjectResult(db.Database.CanConnect());
        }
    }
}
 