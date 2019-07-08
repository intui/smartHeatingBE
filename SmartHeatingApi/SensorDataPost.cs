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
    public static class SensorDataPost
    {
        [HttpPost]
        [FunctionName("SensorDataPost")]
        public static async Task<IActionResult> RunPost(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger POST SensorData function processed a request.");
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                Models.SensorData sensordata;
                try
                {
                    sensordata = (Models.SensorData)JsonConvert.DeserializeObject(requestBody, typeof(Models.SensorData));
                }
                catch (JsonSerializationException ex)
                {
                    log.LogError("Error deserializing Object: " + requestBody);
                    return new BadRequestResult();
                }

                using (SqlConnection connection = Helper.AzureSQLServerHelper.connection)
                {
                    log.LogInformation("\nSQL Query started");
                    log.LogTrace("SensorData submitted: \n" + JsonConvert.SerializeObject(sensordata));

                    var culture = CultureInfo.CreateSpecificCulture("en-US");
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO SensorData (sensorID, tsCreated, SensorDataType, unit, value, valueText) VALUES(");
                    sb.Append(sensordata.SensorID.ToString() + ", ");
                    sb.Append("'" + sensordata.TSCreated.ToUniversalTime().ToString() + "'" + ", ");
                    sb.Append("'" + sensordata.SensorDataType + "'" + ", ");
                    sb.Append("'" + sensordata.Unit + "'" + ", ");
                    sb.Append(sensordata.Value.ToString(culture) + ", ");
                    sb.Append("'" + sensordata.ValueText + "');");
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
                return new BadRequestObjectResult("Error message: " + e.Message);
            }
            return new OkResult();
        }

    }
}
