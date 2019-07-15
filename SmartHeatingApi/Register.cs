using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace SmartHeatingApi
{
    public static class Register
    {
        [FunctionName("Register")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger Register function processed a request.");
            try
            {
                string id = req.Query["id"];
                string pin = req.Query["pin"];

                log.LogInformation("C# HTTP trigger Register function received id and pin: " + id + ", " + pin);

                DateTime centuryBegin = new DateTime(1970, 1, 1);
                long elapsedTicks = DateTime.Now.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                log.LogInformation("Register, seconds since 1970: " + Convert.ToInt32(elapsedSpan.TotalSeconds).ToString());
                return id != null && pin != null
                    ? (ActionResult)new OkObjectResult(Convert.ToInt32(elapsedSpan.TotalSeconds).ToString())
                    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }
            catch (Exception ex)
            {
                log.LogError("Error in register function");
                return new BadRequestResult();
            }
        }
    }
}
