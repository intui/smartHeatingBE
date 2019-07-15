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
using SmartHeatingApi.Helper;
using System.Data.SqlClient;
using System.Text;

namespace SmartHeatingApi
{
    public static class PostDeviceEvent
    {
        [FunctionName("PostDeviceEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger PostAmbience function processed a request.");
            DeviceEvent devEvent;
            try
            {
                //var testEvent = new DeviceEvent { DeviceID = Guid.NewGuid(), DeviceEventID = 1, TSCreated = DateTime.UtcNow, Event = "{\"HeatingValue\": true }" };
                //var testJson = JsonConvert.SerializeObject(testEvent);
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                log.LogInformation("requestBody received: " + requestBody);
                try
                {
                    devEvent = (DeviceEvent)JsonConvert.DeserializeObject(requestBody, typeof(DeviceEvent));
                    log.LogInformation("devEvent deserialized: " + JsonConvert.SerializeObject(devEvent));
                }
                catch (JsonSerializationException ex)
                {
                    log.LogError("Error deserializing Object: " + requestBody);
                    return new BadRequestResult();
                }

                using (SqlConnection connection = AzureSQLServerHelper.Connection)
                {
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    //sb.Append("SELECT TOP 1 * ");sb.Append("FROM [Ambiences]");
                    sb.Append("INSERT INTO DeviceEvents (DeviceID, TSCreated, Event) VALUES(");
                    sb.Append("'" + devEvent.DeviceID + "', ");
                    sb.Append("'" + devEvent.TSCreated.ToUniversalTime() + "', ");
                    sb.Append("'" + devEvent.Event + "');");
                    string sql = sb.ToString();
                    log.LogInformation("trying to execute SQL : " + sql + "\n");
                    log.LogInformation("result: \n");
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                log.LogInformation("{0} {1}", reader.GetDateTime(1).ToString(), Convert.ToString(reader.GetDecimal(2)));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                log.LogError(e, e.Message);
                return new BadRequestObjectResult("SQL error.");
            }
            return new OkObjectResult("o.k.");
        }
    }
}
