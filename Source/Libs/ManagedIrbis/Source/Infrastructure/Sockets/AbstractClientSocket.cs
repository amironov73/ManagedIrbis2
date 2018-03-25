// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractClientSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Threading;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public abstract class AbstractClientSocket
    {
        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; internal set; }

        /// <summary>
        /// Busy state flag.
        /// </summary>
        [NotNull]
        public BusyState Busy { get; private set; }

        /// <summary>
        /// Inner socket.
        /// </summary>
        [CanBeNull]
        public AbstractClientSocket InnerSocket { get; internal set; }

        /// <summary>
        /// Requires connection?
        /// </summary>
        public virtual bool RequireConnection => true;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractClientSocket
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Connection = connection;
            Busy = new BusyState(false);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Abort the request.
        /// </summary>
        public abstract void AbortRequest();

        /// <summary>
        /// Send request to server and receive answer.
        /// </summary>
        /// <exception cref="IrbisNetworkException"></exception>
        [NotNull]
        public abstract byte[] ExecuteRequest
            (
                [NotNull] byte[] request
            );

        #endregion
    }
}
