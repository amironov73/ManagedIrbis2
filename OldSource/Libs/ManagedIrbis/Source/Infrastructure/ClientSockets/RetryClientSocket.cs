// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RetryClientSocket.cs -- retry on network error.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Retry on network error.
    /// </summary>
    [PublicAPI]
    public sealed class RetryClientSocket
        : ClientSocket
    {
        #region Properties

        /// <summary>
        /// Delay between sequent attempts.
        /// </summary>
        public int DelayInterval
        {
            get => RetryManager.DelayInterval;
            set => RetryManager.DelayInterval = value;
        }

        /// <summary>
        /// Retry manager.
        /// </summary>
        [NotNull]
        public RetryManager RetryManager { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public RetryClientSocket
            (
                [NotNull] ClientSocket innerSocket,
                [NotNull] RetryManager retryManager
            )
        {
            Sure.NotNull(innerSocket, nameof(innerSocket));
            Sure.NotNull(retryManager, nameof(retryManager));

            InnerSocket = innerSocket;
            RetryManager = retryManager;
        }

        #endregion

        #region ClientSocket members

        /// <inheritdoc cref="ClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ClientSocket.ExecuteRequest" />
        public override void ExecuteRequest
            (
                ClientContext context
            )
        {
            ClientSocket innerSocket = InnerSocket.ThrowIfNull(nameof(InnerSocket));
            Action<ClientContext> action = innerSocket.ExecuteRequest;

            RetryManager.Try(action, context);
        }

        #endregion
    }
}
