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
using System.Globalization;

namespace SmartHeatingApi
{
    public static class GetActiveProgram
    {
        [FunctionName("GetActiveProgram")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function ProgramGetActive processed a request.");

                string deviceId = req.Query["DeviceId"];
                string result = string.Empty;
                if (deviceId != null)
                {
                    using (SqlConnection connection = Helper.AzureSQLServerHelper.Connection)
                    {
                        log.LogInformation("\nSQL Query started");
                        log.LogTrace("sensorId submitted: \n" + deviceId);
                        log.LogInformation("Sql connection.DataSource: " + connection?.DataSource);

                        var culture = CultureInfo.CreateSpecificCulture("en-US");
                        connection.Open();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT TOP 1 ProgramContent FROM Programs WHERE DeviceId = '" + deviceId + "' AND Active = 1 ORDER BY ValidFrom DESC");
                        string sql = sb.ToString();
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
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);
                throw new Exception("Exception", ex);
            }
        }
    }
}
