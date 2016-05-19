using System;
using System.Data;
using System.Data.SqlClient;

namespace SqlHelper.Abstract
{
    /// <summary>
    /// Abstract Class to Connect with SQL Server
    /// <para>This Class <see cref="SqlDbHelperConnection"/> has inherit in <see cref="SqlDbTypeHelper"/> with some functionalities.</para>
    /// </summary>
    public abstract class SqlDbHelperConnection
        : SqlDbTypeHelper
    {
        #region Constructors

        protected SqlDbHelperConnection(string aplicacao, string servidor, string banco, string usuario, string senha, int connectionTimeOut)
        {
            this.Aplicacao = aplicacao;
            this.Servidor = servidor;
            this.Banco = banco;
            this.Usuario = usuario;
            this.Senha = senha;
            this.ConnectionTimeOut = connectionTimeOut;
            this.ConnectionString = CreateAndSetDbConnectionString().ConnectionString;

            Initialize();
        }

        #endregion Constructors

        #region Protected Properties

        protected string Aplicacao { get; private set; }
        protected string Servidor { get; private set; }
        protected string Banco { get; private set; }
        protected string Usuario { get; private set; }
        protected string Senha { get; private set; }
        protected int ConnectionTimeOut { get; private set; }
        protected string ConnectionString { get; private set; }

        #endregion Protected Properties

        #region Public Properties

        public SqlConnection CnBanco { get { return GetConnectionStatusAndTryOpenIfItsClosedOrBroken(); } }

        public bool IsConnected { get { return CheckConnectionState(); } }

        #endregion Public Properties

        #region Protected Methods

        protected abstract void Initialize();

        #endregion Protected Methods

        #region Private Methods

        private bool CheckConnectionState()
        {
            return ((CnBanco.State == ConnectionState.Open) || (CnBanco.State == ConnectionState.Executing) || (CnBanco.State == ConnectionState.Fetching));
        }

        private SqlConnectionStringBuilder CreateAndSetDbConnectionString()
        {
            return new SqlConnectionStringBuilder()
            {
                ApplicationName = Aplicacao,
                DataSource = Servidor,
                InitialCatalog = Banco,
                UserID = Usuario,
                Password = Senha,
                ConnectTimeout = ConnectionTimeOut,
                PersistSecurityInfo = true,
                MultipleActiveResultSets = true
            };
        }

        private SqlConnection GetConnectionStatusAndTryOpenIfItsClosedOrBroken()
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(ConnectionString);
                if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
                    conn.Open();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            return conn;
        }

        #endregion Private Methods
    }
}