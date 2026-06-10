using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Infrastructure.SqlServer.Common
{
    public static class DataRowExtensions
    {
        public static string GetString(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? null
                : row[columnName].ToString();
        }

        public static int? GetInt(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? (int?)null
                : Convert.ToInt32(row[columnName]);
        }

        public static long? GetLong(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? (long?)null
                : Convert.ToInt64(row[columnName]);
        }

        public static double? GetDouble(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? (double?)null
                : Convert.ToDouble(row[columnName]);
        }

        public static decimal? GetDecimal(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? (decimal?)null
                : Convert.ToDecimal(row[columnName]);
        }

        public static DateTime? GetDateTime(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value
                ? (DateTime?)null
                : Convert.ToDateTime(row[columnName]);
        }
    }
}
