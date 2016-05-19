using SqlHelper.Abstract;

namespace SqlHelper
{
    public class SqlDb :
        SqlDbHelper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDb"></see> class.
        /// </summary>
        public SqlDb(string aplicativo, string servidor, string banco, string usuario, string senha, int connectionTimeOut = 120)
            : base(aplicativo, servidor, banco, usuario, senha, connectionTimeOut)
        {
        }

        #endregion Constructors
    }
}