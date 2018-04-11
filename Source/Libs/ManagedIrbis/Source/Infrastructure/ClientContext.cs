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
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Infrastructure.ClientCommands;
using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure
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

        #region Public methods

        /// <summary>
        /// Whether the connection already established.
        /// </summary>
        public void CheckAlreadyConnected()
        {
            if (Connection.Connected)
            {
                Log.Error
                    (
                        nameof(ClientContext) + "::" + nameof(CheckAlreadyConnected)
                        + ": " + Resources.AlreadyConnected
                    );

                throw new IrbisException(Resources.IrbisConnection_AlreadyConnected);
            }
        }

        /// <summary>
        /// Get database setting for the context.
        /// </summary>
        [NotNull]
        public string GetDatabase
            (
                [CanBeNull] string database
            )
        {
            string result = database.IfEmpty(Connection.Database);
            if (ReferenceEquals(result, null) || result.Length == 0)
            {
                Log.Error
                    (
                        nameof(ClientContext) + "::" + nameof(GetDatabase)
                        + ": " + Resources.DatabaseNotSet
                    );

                throw new IrbisException(Resources.DatabaseNotSet);
            }

            return result;
        }

        /// <summary>
        /// Get database setting for the context.
        /// </summary>
        [NotNull]
        public string GetUsername
            (
                [CanBeNull] string username
            )
        {
            string result = username.IfEmpty(Connection.Username);
            if (ReferenceEquals(result, null) || result.Length == 0)
            {
                Log.Error
                    (
                        nameof(ClientContext) + "::" + nameof(GetUsername)
                        + ": " + Resources.UsernameNotSpecified
                    );

                throw new IrbisException(Resources.UsernameNotSpecified);
            }

            return result;
        }

        /// <summary>
        /// Get password setting for the context.
        /// </summary>
        [NotNull]
        public string GetPassword
            (
                [CanBeNull] string password
            )
        {
            string result = password.IfEmpty(Connection.Password);
            if (ReferenceEquals(result, null) || result.Length == 0)
            {
                Log.Error
                    (
                        nameof(ClientContext) + "::" + nameof(GetPassword)
                        + ": " + Resources.UsernameNotSpecified
                    );

                throw new IrbisException(Resources.PasswordNotSpecified);
            }

            return result;
        }

        #endregion
    }
}
