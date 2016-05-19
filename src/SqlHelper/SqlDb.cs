using SqlHelper.Abstract;

namespace SqlHelper
{

    /// <summary>
    /// SqlDb Class Help's you connect and execute SqlCommands
    /// <para>
    /// <see cref="!:https://github.com/elioseverojunior/SqlHelper">this</see> MSDN-Link.
    ///     AHref <a href="https://github.com/elioseverojunior/SqlHelper">here</a>.
    ///     see-href <see href="https://github.com/elioseverojunior/SqlHelper">here</see>.
    /// </para>
    /// </summary>
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