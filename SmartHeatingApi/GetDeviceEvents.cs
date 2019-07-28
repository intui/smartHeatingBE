using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using SmartHeatingApi.Models;
using System.Data.SqlClient;
using System.Text;

namespace SmartHeatingApi
{
    public static class GetDeviceEvents
    {
        [FunctionName("GetDeviceEvents")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            List<DeviceEvent> result = new List<DeviceEvent>();
            try
            {
                log.LogInformation("C# HTTP trigger function GetDeviceEvents processed a request.");
                string deviceId = req.Query["deviceId"];
                string dateFrom = req.Query["dateFrom"];

                using (SqlConnection connection = Helper.AzureSQLServerHelper.Connection)
                {
                    log.LogInformation("SQL connection established.");
                    log.LogInformation(connection.DataSource);

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM DeviceEvents WHERE DeviceId = '" + deviceId + "' AND TSCreated > '" + dateFrom + "' ORDER BY TSCreated"); // DESC");
                    string sql = sb.ToString();
                    log.LogInformation("trying to execute SQL : " + sql + "\n");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    result.Add(new DeviceEvent
                                    {
                                        DeviceEventID = (int)reader["DeviceEventID"],
                                        DeviceID = (Guid)reader["DeviceID"],
                                        TSCreated = (DateTime)reader["TSCreated"],
                                        Event = (string)reader["Event"]
                                    });
                                }
                                catch (Exception ex)
                                {
                                    log.LogError(ex.Message);
                                }
                                log.LogInformation("result: " + result);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }
            return new OkObjectResult(result);
        }
    }
}
