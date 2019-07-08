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
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = Environment.GetEnvironmentVariable("DataSource"),
                    UserID = Environment.GetEnvironmentVariable("UserID"),
                    Password = Environment.GetEnvironmentVariable("Password"),
                    InitialCatalog = Environment.GetEnvironmentVariable("InitialCatalog")
                };
                return new SqlConnection(builder.ConnectionString);
            }
        }
    }
}
