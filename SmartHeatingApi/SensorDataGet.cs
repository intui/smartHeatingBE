using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartHeatingApi.Models;
using System.Data.SqlClient;

namespace SmartHeatingApi
{
    public static class SensorDataGet
    {
        [HttpGet]
        [FunctionName("SensorDataGet")]
        public static async Task<IActionResult> RunGet(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger Sensordata GET processed a request.");

            string sensorId = req.Query["sensorId"];

            using (SqlConnection connection = Helper.AzureSQLServerHelper.Connection)
            {
                log.LogInformation("SQL connection established.");
                log.LogInformation(connection.DataSource);
            }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            sensorId = sensorId ?? data?.sensorId;
            SensorData retVal = new SensorData { SensorDataID = 0, SensorID = 007, TSCreated = DateTime.UtcNow, SensorDataType = 1, Unit = "°C", Value = 23.2M, ValueText = "Temperature" } ;
            return sensorId != null
                ? (ActionResult)new OkObjectResult(retVal)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
