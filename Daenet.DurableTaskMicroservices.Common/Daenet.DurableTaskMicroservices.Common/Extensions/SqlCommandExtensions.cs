using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Daenet.Common.Logging;

namespace Daenet.DurableTaskMicroservices.Common.Extensions
{
    public static class SqlCommandExtensions
    {
        #region Public Fields
        public static int m_UniqueParamID = 0;
        #endregion

        #region Public Methods
        public static SqlParameter AddUniqueSqlParameter(this SqlCommand command, string parameter, object value)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = parameter + (m_UniqueParamID++).ToString();
            sqlParameter.Value = value;
            if (value == null)
            {
                sqlParameter.Value = DBNull.Value;
            }
            command.Parameters.Add(sqlParameter);
            return sqlParameter;
        }

        public static SqlParameter AddSqlParameter(this SqlCommand command, string parameter, object value)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = parameter;
            sqlParameter.Value = value;
            if (value == null)
            {
                sqlParameter.Value = DBNull.Value;
            }
            command.Parameters.Add(sqlParameter);
            return sqlParameter;
        }

        public static SqlParameter AddSqlParameter(this SqlCommand command, string parameter, DbType dbType, object value)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = parameter;
            sqlParameter.Value = value;
            sqlParameter.DbType = dbType;
            if (value == null)
            {
                sqlParameter.Value = DBNull.Value;
            }
            command.Parameters.Add(sqlParameter);
            return sqlParameter;
        }

        public static void LogSqlStatement(this SqlCommand command, ILogManager log, string message)
        {
            log.TraceMessage(TracingLevel.Level4, 0, "{0}", new Func<string>(() =>
            {
                
                return String.Format(message + Environment.NewLine+   "SQL ({0}): {1}" + Environment.NewLine + "Parameters: {2}",
                    command.CommandType,
                    command.CommandText,
                    string.Join(",", command.Parameters.OfType<SqlParameter>().Select(a => a.ParameterName + ":" + a.Value)));
            }));
        }

        public static DateTime AdjustToSqlMinMax(this DateTime date)
        {
            var t =  AdjustToSqlMinMax(new Nullable<DateTime>(date));
            return t.Value;
        }
        public static DateTime? AdjustToSqlMinMax(this DateTime? date)
        {
            if (date == DateTime.MinValue)
                return new DateTime(1900, 1, 1);
            else if (date == DateTime.MaxValue)
                return new DateTime(2500, 1, 1);
            else
                return date;

        }
        #endregion
    }
}
