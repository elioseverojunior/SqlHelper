using System;
using System.Collections.Generic;
using System.Data;

namespace SqlHelper.Abstract
{
    /// <summary>
    /// Abstract Class to Help Trace <see cref="SqlDbType"/>
    /// </summary>
    public abstract class SqlDbTypeHelper
    {
        private static Dictionary<Type, SqlDbType> typeMap;

        public SqlDbTypeHelper()
        {
            typeMap = new Dictionary<Type, SqlDbType>();

            typeMap[typeof(string)] = SqlDbType.NVarChar;
            typeMap[typeof(char[])] = SqlDbType.NVarChar;
            typeMap[typeof(byte)] = SqlDbType.TinyInt;
            typeMap[typeof(short)] = SqlDbType.SmallInt;
            typeMap[typeof(int)] = SqlDbType.Int;
            typeMap[typeof(long)] = SqlDbType.BigInt;
            typeMap[typeof(byte[])] = SqlDbType.Image;
            typeMap[typeof(bool)] = SqlDbType.Bit;
            typeMap[typeof(DateTime)] = SqlDbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            typeMap[typeof(decimal)] = SqlDbType.Decimal;
            typeMap[typeof(float)] = SqlDbType.Real;
            typeMap[typeof(double)] = SqlDbType.Float;
            typeMap[typeof(TimeSpan)] = SqlDbType.Time;
        }

        public SqlDbType GetDbType(Type giveType)
        {
            if (typeMap.ContainsKey(giveType))
            {
                return typeMap[giveType];
            }

            throw new ArgumentException($"{giveType.FullName} não é suportado pela Classe {this.GetType().FullName} .NET.");
        }

        public SqlDbType GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }
}