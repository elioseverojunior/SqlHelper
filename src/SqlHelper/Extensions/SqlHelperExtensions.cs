using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlHelper.Extensions
{
    public static class SqlHelperExtensions
    {
        public static void Parameterize(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        public static T To<T>(this IDataRecord dr, int index, T defaultValue = default(T),
                              Func<object, T> converter = null)
        {
            return dr[index].To<T>(defaultValue, converter);
        }

        public static T To<T>(this IDataRecord dr, string column, T defaultValue = default(T),
                              Func<object, T> converter = null)
        {
            return dr[column].To<T>(defaultValue, converter);
        }

        private static T To<T>(this object obj, T defaultValue, Func<object, T> converter)
        {
            if (obj.IsNull())
                return defaultValue;

            return converter == null ? (T)obj : converter(obj);
        }

        public static bool IsNull<T>(this T obj) where T : class
        {
            return (object)obj == null || obj == DBNull.Value;
        }
    }
}