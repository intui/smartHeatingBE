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
using System.Globalization;
using System.Text;

namespace SmartHeatingApi
{
    public static class ProgramPost
    {
        [FunctionName("ProgramPost")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function ProgramPost processed a request.");
            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                Models.Program program;
                try
                {
                    program = (Models.Program)JsonConvert.DeserializeObject(requestBody, typeof(Models.Program));
                }
                catch (JsonSerializationException ex)
                {
                    log.LogError("Error deserializing Object: " + requestBody);
                    return new BadRequestResult();
                }

                using (SqlConnection connection = Helper.AzureSQLServerHelper.Connection)
                {
                    log.LogInformation("\nSQL Query started");
                    log.LogTrace("program submitted: \n" + JsonConvert.SerializeObject(program));

                    var culture = CultureInfo.CreateSpecificCulture("en-US");
                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO Programs (ProgramText, ProgramType, Active, ValidFrom, ValidTo, ProgramContent, DeviceID, TSCreated, TSUpdated) VALUES(");
                    sb.Append("'" + program.ProgramText + "'" + ", ");
                    sb.Append(program.ProgramType.ToString() + ", ");
                    if(program.Active)
                        sb.Append("1, ");
                    else
                        sb.Append("0, ");
                    sb.Append("'" + program.ValidFrom.ToUniversalTime().ToString() + "'" + ", ");
                    sb.Append("'" + program.ValidTo.ToUniversalTime().ToString() + "'" + ", ");
                    sb.Append("'" + program.ProgramContent + "'" + ", ");
                    sb.Append(program.DeviceID.ToString() + ", ");
                    sb.Append("'" + program.TSCreated.ToUniversalTime().ToString() + "'" + ", ");
                    sb.Append("'" + program.TSUpdated.ToUniversalTime().ToString() + "');");

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
