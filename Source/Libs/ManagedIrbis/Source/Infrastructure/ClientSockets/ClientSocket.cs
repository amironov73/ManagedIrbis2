// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* AbstractClientSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM.Threading;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public abstract class ClientSocket
    {
        #region Properties

        /// <summary>
        /// Busy state flag.
        /// </summary>
        [NotNull]
        public BusyState Busy { get; } = new BusyState(false);

        /// <summary>
        /// Inner socket.
        /// </summary>
        [CanBeNull]
        public ClientSocket InnerSocket { get; internal set; }

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
        public abstract void ExecuteRequest
            (
                [NotNull] ClientContext context
            );

        #endregion
    }
}
