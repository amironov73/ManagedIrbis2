// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ClientContext.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Контекст формирования и отправки клиентской команды,
    /// а также получения и парсинга ответа сервера.
    /// </summary>
    [PublicAPI]
    public sealed class ClientContext
    {
        #region Properties

        /// <summary>
        /// Connection.
        /// </summary>
        [NotNull]
        public IIrbisConnection Connection { get; }

        /// <summary>
        /// Command to execute.
        /// </summary>
        [CanBeNull]
        public ClientCommand Command { get; set; }

        /// <summary>
        /// Client query.
        /// </summary>
        [CanBeNull]
        public ClientQuery Query { get; set; }

        /// <summary>
        /// Raw client request content.
        /// </summary>
        [CanBeNull]
        public byte[] RawQuery { get; set; }

        /// <summary>
        /// Server response.
        /// </summary>
        [CanBeNull]
        public ServerResponse Response { get; set; }

        /// <summary>
        /// Raw server response content.
        /// </summary>
        [CanBeNull]
        public byte[] RawResponse { get; set; }

        /// <summary>
        /// Exception.
        /// </summary>
        [CanBeNull]
        public Exception Exception { get; set; }

        /// <summary>
        /// Exception handled?
        /// </summary>
        public bool ExceptionHandled { get; set; }

        /// <summary>
        /// Arbitrary user data.
        /// </summary>
        [CanBeNull]
        public object UserData { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClientContext
            (
                [NotNull] IIrbisConnection connection
            )
        {
            Sure.NotNull(connection, nameof(connection));

            Connection = connection;
        }

        #endregion
    }
}
