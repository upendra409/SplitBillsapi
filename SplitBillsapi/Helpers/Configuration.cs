using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitBillsapi.Helpers
{
    public static class Configuration
    {
        public static string connectionString { get; set; }
        public static string GetConnectionString(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("TestDatabase1");
            return connectionString;
        }
    }
}
