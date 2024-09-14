using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace AspNetCoreDemo.Common.Extensions
{
    /// <summary>
    /// 数据库扩展方法
    /// </summary>
    public static class DbContextExtensions
    {
        #region 私有方法
        private static void CombineParams(ref DbCommand command, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    if (!parameter.ParameterName.Contains("@"))
                        parameter.ParameterName = $"@{parameter.ParameterName}";

                    if (parameter.ParameterName.Contains("@pagestart", StringComparison.OrdinalIgnoreCase) && parameter.Value is int start && start < 0)
                        parameter.Value = 0;
                    command.Parameters.Add(parameter);
                }
            }
        }

        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection dbConn, params object[] parameters)
        {
            DbConnection conn = facade.GetDbConnection();
            dbConn = conn;
            conn.Open();
            DbCommand cmd = conn.CreateCommand();
            if (facade.IsMySql())
            {
                cmd.CommandText = sql;
                CombineParams(ref cmd, parameters);
            }
            return cmd;
        }
        #endregion

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            DbCommand cmd = CreateCommand(facade, sql, out DbConnection conn, parameters);
            DbDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt;
        }

        public static Object SqlQueryCount(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            DbCommand cmd = CreateCommand(facade, sql, out DbConnection conn, parameters);
            Object num = cmd.ExecuteScalar();
            conn.Close();
            return num;
        }

        public static IEnumerable<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            DataTable dt = SqlQuery(facade, sql, parameters);
            return dt.ToEnumerable<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new()
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                T t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        if (p.GetType() == typeof(DateTime))
                        {
                            p.SetValue(t, row[p.Name].ToString(), null);
                        }
                        else
                        {
                            p.SetValue(t, row[p.Name], null);
                        }

                }
                ts[i] = t;
                i++;
            }
            return ts;
        }
    }
}
