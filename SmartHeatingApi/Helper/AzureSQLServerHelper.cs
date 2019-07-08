using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
//using System.Environment;

namespace SmartHeatingApi.Helper
{
    public class AzureSQLServerHelper
    {
        public static SqlConnection Connection
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = Environment.GetEnvironmentVariable("DataSource");
                builder.UserID = Environment.GetEnvironmentVariable("UserID");
                builder.Password = Environment.GetEnvironmentVariable("Password");
                builder.InitialCatalog = Environment.GetEnvironmentVariable("InitialCatalog");
                return new SqlConnection(builder.ConnectionString);
            }
        }
    }
}
