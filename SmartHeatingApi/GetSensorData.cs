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
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace SmartHeatingApi
{
    public static class GetSensorData
    {
        [HttpGet]
        [FunctionName("GetSensorData")]
        public static async Task<IActionResult> RunGet(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            List<SensorData> result = new List<SensorData>();
            try
            {
                log.LogInformation("C# HTTP trigger Sensordata GET processed a request.");
                string sensorId = req.Query["sensorId"];
                string dateFrom = req.Query["dateFrom"];
                // dateTo notYetImplemented
                using (SqlConnection connection = Helper.AzureSQLServerHelper.Connection)
                {
                    log.LogInformation("SQL connection established.");
                    log.LogInformation(connection.DataSource);

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT * FROM SensorData WHERE SensorId = '" + sensorId + "' AND TSCreated > '" + dateFrom + "' ORDER BY TSCreated"); // DESC");
                    string sql = sb.ToString();
                    log.LogInformation("trying to execute SQL : " + sql + "\n");

                    log.LogInformation("result: \n");

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    result.Add(new SensorData
                                    {
                                        SensorDataID = (int)reader["SensorDataID"],
                                        SensorDataType = (int)reader["SensorDataType"],
                                        SensorID = (Guid)reader["SensorID"],
                                        TSCreated = (DateTime)reader["TSCreated"],
                                        Unit = (string)reader["Unit"],
                                        Value = (decimal) Convert.ToSingle(reader["Value"], new CultureInfo("en-US")),
                                        ValueText = (string)reader["ValueText"]
                                    });
                                }
                                catch(Exception ex)
                                {
                                    log.LogError(ex.Message);
                                }
                                log.LogInformation("result: " + result);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return new BadRequestResult();
            }
            return new OkObjectResult(result);
        }
    }
}
