
using Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;



namespace Functions.Test
{
    public class PopulateDb
    {
        private readonly ILogger<PopulateDb> _logger;

        private readonly UtilService utilService;

        public PopulateDb(ILogger<PopulateDb> logger, UtilService utilService)
        {
            _logger = logger;
            this.utilService = utilService;

        }

        [Function("PopulateDb")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PopulateDb")] HttpRequest req, FunctionContext executionContext)
        {
            try
            {
                bool result = await utilService.InitDb();
                if (result)
                    return new OkObjectResult("Database successfully initialized");
                return new BadRequestObjectResult("");
            }
            catch (Exception ex)
            {
                return RequestService.GetErrorResult(ex);
            }

        }
    }
}
