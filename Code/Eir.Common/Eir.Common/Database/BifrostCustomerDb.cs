using System;
using System.Configuration;

namespace Eir.Common.Database
{
    public static class BifrostCustomerDb
    {
        public static string GetConnectionString(string dbHostName, string sysname)
        {
            return GetConnectionStringFormat()
                .ValidatedReplace("[CUSTOMERDBHOSTNAME]", dbHostName)
                .ValidatedReplace("[CUSTOMERSYSNAME]", sysname);
        }

        public static string GetBifrostConnectionString(string dbHostName)
        {
            return GetConnectionStringFormat()
                .ValidatedReplace("[CUSTOMERDBHOSTNAME]", dbHostName)
                .ValidatedReplace("bifrost_[CUSTOMERSYSNAME]", "bifrost");
        }

        private static string GetConnectionStringFormat()
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["BifrostCustomerDb"];
            if (connectionStringSettings == null)
            {
                throw new InvalidOperationException("The BifrostCustomerDb connection string is not configured for this application!");
            }

            return connectionStringSettings.ConnectionString;
        }

        private static string ValidatedReplace(this string text, string placeholder, string value)
        {
            int index = text.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                throw new FormatException($"The placeholder \"{placeholder}\" was not found in the bifrost customer DB connection string configuration!");
            }

            return text.Remove(index, placeholder.Length).Insert(index, value);
        }
    }
}