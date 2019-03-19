// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Threading.Tasks;

using JetBrains.Annotations;

using AM;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class IrbisSocket
    {
        #region Properties

        /// <summary>
        /// Connection
        /// </summary>
        [NotNull]
        public IrbisConnection Connection { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        protected IrbisSocket
            (
                [NotNull] IrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Connection = connection;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public abstract Task<ServerResponse> Transact(ClientQuery query);

        #endregion
    }
}
