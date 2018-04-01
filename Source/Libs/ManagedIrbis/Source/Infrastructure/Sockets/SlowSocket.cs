// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SlowSocket.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Threading;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Сокет, сознательно замедляющий связь с сервером
    /// (для тестирования пользовательского интерфейса).
    /// </summary>
    [PublicAPI]
    public sealed class SlowSocket
        : AbstractClientSocket
    {
        #region Constants

        /// <summary>
        /// Default value for <see cref="Delay"/>.
        /// </summary>
        public const int DefaultDelay = 300;

        #endregion

        #region Properties

        /// <summary>
        /// Delay, milliseconds.
        /// </summary>
        public int Delay { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public SlowSocket
            (
                [NotNull] IrbisConnection connection,
                [NotNull] AbstractClientSocket innerSocket
            )
            : base(connection)
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNull(innerSocket, nameof(innerSocket));

            Delay = DefaultDelay;
            InnerSocket = innerSocket;
        }

        #endregion

        #region AbstractClientSocket members

        /// <inheritdoc cref="AbstractClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            InnerSocket.ThrowIfNull().AbortRequest();
        }

        /// <inheritdoc cref="AbstractClientSocket.ExecuteRequest" />
        public override byte[] ExecuteRequest
            (
                byte[] request
            )
        {
            Sure.NotNull(request, nameof(request));

            int delay = Delay;
            if (delay > 0)
            {
                Thread.Sleep(delay);
            }

            byte[] result = InnerSocket.ThrowIfNull(nameof(InnerSocket))
                .ExecuteRequest(request);

            return result;
        }

        #endregion
    }
}
