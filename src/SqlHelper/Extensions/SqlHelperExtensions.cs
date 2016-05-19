using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else if (row[property.Name].GetType() == typeof(System.DBNull))
                {
                    property.SetValue(item, "NULL", null);
                }
                else
                {
                    if ("Coluna".Equals(property.Name))
                    {
                        property.SetValue(item, row[property.Name].ToString().Capitalize('_'), null);
                    }
                    else
                    {
                        property.SetValue(item, row[property.Name], null);
                    }
                }
            }
            return item;
        }
    }
}