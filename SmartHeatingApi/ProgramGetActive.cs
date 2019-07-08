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
using System.Text;

namespace SmartHeatingApi
{
    public static class ProgramGetActive
    {
        [FunctionName("ProgramGetActive")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ProgramGetActive processed a request.");

            string deviceId = req.Query["DeviceId"];
            string result = string.Empty;
            if (deviceId != null)
            {
                using (SqlConnection connection = Helper.AzureSQLServerHelper.connection)
                {
                    log.LogInformation("\nSQL Query started");
                    log.LogTrace("sensorId submitted: \n" + deviceId);

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    //sb.Append("SELECT TOP 1 * ");sb.Append("FROM [Ambiences]");
                    sb.Append("SELECT TOP 1 ProgramContent FROM Programs WHERE DeviceId = '" + deviceId + "' ORDER BY ValidFrom DESC");
                    String sql = sb.ToString();
                    log.LogInformation("trying to execute SQL : " + sql + "\n");

                    log.LogInformation("result: \n");
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result = reader.GetString(0);
                                log.LogInformation("result: " + result);
                            }
                        }
                    }
                }
                return new OkObjectResult(result);
            }
            else
            {
                return new BadRequestObjectResult("Please pass the DeviceId on the query string");
            }
        }
    }
}
