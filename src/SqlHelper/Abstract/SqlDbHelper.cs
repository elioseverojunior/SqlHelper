using SqlHelper.Class;
using SqlHelper.Extensions;
using SqlHelper.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SqlHelper.Abstract
{
    public abstract class SqlDbHelper
        : SqlDbHelperConnection

    {
        public SqlDbHelper(string aplicacao, string servidor, string banco, string usuario, string senha, int connectionTimeOut = 120)
            : base(aplicacao, servidor, banco, usuario, senha, connectionTimeOut)
        {
        }

        #region Public Properties

        public List<PrimaryKey> DbKeys { get; private set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void Initialize()
        {
            //DbKeys = new List<PrimaryKey>();
            //GetAllPrimaryKeysFromDatabaseTables();
        }

        #endregion Protected Methods

        #region Private Methods

        private void GetAllPrimaryKeysFromDatabaseTables()
        {
            DbKeys = Get(Resources.queryPK, null,
                            r => new PrimaryKey(
                                        r.To<string>("Tabela"),
                                        r.To<string>("Coluna"),
                                        r.To<bool>("Identity")
                            )).ToList();
        }

        #endregion Private Methods

        #region Public Methods

        #region Add Parameter To Query

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="size">The size.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="sourceVersion">The source version.</param>
        /// <param name="value">The value.</param>
        private void AddParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter p = new SqlParameter(parameterName, dbType, size, direction, precision, scale, sourceColumn,
                sourceVersion, true, value, null, null, null);
            command.Parameters.Add(p);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="size">The size.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The value.</param>
        public void AddParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size, ParameterDirection direction, object value)
        {
            AddParameter(command, parameterName, dbType, size, direction, 0, 0, null, DataRowVersion.Current, value);
        }

        /// <summary>
        /// Adds the in parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="value">The value.</param>
        public void AddInParameter(SqlCommand command, string parameterName, SqlDbType dbType, object value)
        {
            AddParameter(command, parameterName, dbType, 0, ParameterDirection.Input, value);
        }

        /// <summary>
        /// Adds the out parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="size">The size.</param>
        public void AddOutParameter(SqlCommand command, string parameterName, SqlDbType dbType, int size)
        {
            AddParameter(command, parameterName, dbType, size, ParameterDirection.Output, null);
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public object GetParameterValue(SqlCommand command, string parameterName)
        {
            return command.Parameters[parameterName].Value;
        }

        #endregion Add Parameter To Query

        #region Generating SqlCommand

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="commandType">Type of the command.
        /// <param name="commandText">The command text.
        /// <returns>Return <see cref="SqlCommand"/> result.</returns>
        private SqlCommand PrepareCommand(CommandType commandType, string commandText)
        {
            if (CnBanco.State == ConnectionState.Closed || CnBanco.State == ConnectionState.Broken)
            {
                CnBanco.Open();
            }
            SqlCommand command = new SqlCommand(commandText, CnBanco);
            command.CommandType = commandType;
            return command;
        }

        public IEnumerable<IDataRecord> GetData(string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, CnBanco))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        yield return rdr;
                    }
                }
            }
        }

        /// <summary>
        /// Get Data
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="query">Query SQL</param>
        /// <param name="parameterizer">Parameterizer <see cref="Extensions.SqlHelperExtensions.Parameterize(IDbCommand, string, object)"/>.</param>
        /// <param name="selector">Selector</param>
        /// <returns>Return List.</returns>
        public IEnumerable<T> Get<T>(string query, Action<IDbCommand> parameterizer,
                             Func<IDataRecord, T> selector)
        {
            using (SqlConnection conn = CnBanco)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (parameterizer != null)
                        parameterizer(cmd);
                    cmd.CommandText = query;
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            yield return selector(r);
                }
            }
        }

        /// <summary>
        /// Gets the SQL query command.
        /// </summary>
        /// <param name="query">The query.
        /// <returns></returns>
        public SqlCommand GetSqlQueryCommand(string query)
        {
            return PrepareCommand(CommandType.Text, query);
        }

        /// <summary>
        /// Gets the store procedure command.
        /// </summary>
        /// <param name="spname">The spname.
        /// <returns>Return <see cref="object"/> as result.</returns>
        public SqlCommand GetStoreProcedureCommand(string spname)
        {
            return PrepareCommand(CommandType.StoredProcedure, spname);
        }

        #endregion Generating SqlCommand

        #region Database Related Command

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Return <see cref="int"/> as result.</returns>
        public int ExecuteNonQuery(SqlCommand command)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Return <see cref="object"/> as result.</returns>
        public object ExecuteScalar(SqlCommand command)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            return command.ExecuteScalar();
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Return <see cref="SqlDataReader"/> with all results.</returns>
        public SqlDataReader ExecuteReader(SqlCommand command)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private void CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(SqlCommand command)
        {
            if (command.Connection == null)
                command.Connection = CnBanco;
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="command">The command.
        /// <param name="commandBehavior">The command behavior.
        /// <returns>Return <see cref="SqlDataReader"/> with all results.</returns>
        public SqlDataReader ExecuteReader(SqlCommand command, CommandBehavior commandBehavior)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            return command.ExecuteReader(commandBehavior);
        }

        /// <summary>
        /// Loads the data table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns><see cref="DataTable"/> with all results.</returns>
        public DataTable LoadDataTable(SqlCommand command, string tableName)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            using (SqlDataAdapter da = new SqlDataAdapter(command))
            {
                using (DataTable dt = new DataTable(tableName))
                {
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Loads the data set.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="tableNames">The table names.</param>
        /// <returns>Return <see cref="DataSet"/> with results</returns>
        public DataSet LoadDataSet(SqlCommand command, string[] tableNames)
        {
            CheckAndSetSqlConnectionIfSqlCommandDoesNotExists(command);
            using (SqlDataAdapter da = new SqlDataAdapter(command))
            {
                using (DataSet ds = new DataSet())
                {
                    da.Fill(ds);
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        ds.Tables[i].TableName = tableNames[i];
                    }
                    return ds;
                }
            }
        }

        #endregion Database Related Command

        #region Transactions

        /// <summary>
        /// Prepares the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.
        /// <returns></returns>
        private SqlTransaction PrepareTransaction(IsolationLevel isolationLevel)
        {
            if (CnBanco.State == ConnectionState.Closed || CnBanco.State == ConnectionState.Broken)
                CnBanco.Open();

            return CnBanco.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public SqlTransaction BeginTransaction()
        {
            return PrepareTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.
        /// <returns></returns>
        public SqlTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return PrepareTransaction(isolationLevel);
        }

        /// <summary>
        /// Commits the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.
        public void Commit(SqlTransaction transaction)
        {
            if (transaction != null)
                transaction.Commit();
        }

        /// <summary>
        /// Rolls the back.
        /// </summary>
        /// <param name="transaction">The transaction.
        public void RollBack(SqlTransaction transaction)
        {
            if (transaction != null)
                transaction.Rollback();
        }

        #endregion Transactions

        #endregion Public Methods
    }
}