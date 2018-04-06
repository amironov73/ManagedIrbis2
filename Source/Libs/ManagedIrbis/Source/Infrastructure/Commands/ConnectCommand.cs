// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConnectCommand.cs -- connect to the IRBIS64 server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    //
    // EXTRACT FROM OFFICIAL DOCUMENTATION
    //
    // Если код возврата равен ZERO,
    // то следующие строки - это ini-файл определенный
    // на сервере для данного пользователя.
    //
    // Если код возврата не равен ZERO - только одна строка.
    //
    // Коды возврата:
    // ZERO
    // CLIENT_ALREADY_EXISTS  - пользователь
    // уже зарегистрирован.
    // WRONG_PASSWORD - неверный пароль.
    //

    /// <summary>
    /// Connect to the IRBIS64 server.
    /// </summary>
    [PublicAPI]
    public class ConnectCommand
        : AbstractCommand
    {
        #region Constants

        /// <summary>
        /// Response specification.
        /// </summary>
        public const string ResponseSpecification = "AIIIAAAAAAT";

        #endregion

        #region Properties

        /// <summary>
        /// Server configuration file content
        /// (on successful connection).
        /// </summary>
        [CanBeNull]
        public string Configuration { get; set; }

        /// <summary>
        /// User password. If not specified,
        /// connection password used.
        /// </summary>
        [CanBeNull]
        public string Password { get; set; }

        /// <summary>
        /// User name. If not specified,
        /// connection name used.
        /// </summary>
        [CanBeNull]
        public string Username { get; set; }

        /// <summary>
        /// Confirmation interval, minutes.
        /// </summary>
        public int ConfirmationInterval { get; set; }

        /// <summary>
        /// Server version.
        /// </summary>
        [CanBeNull]
        public string ServerVersion { get; set; }

        /// <summary>
        /// Doesn't require connection.
        /// </summary>
        public override bool RequireConnection
        {
            get { return false; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ConnectCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
            Log.Trace("ConnectCommand::Constructor");

            // TODO fix it!
            IrbisConnection nativeConnection = connection as IrbisConnection;
            if (!ReferenceEquals(nativeConnection, null))
            {
                nativeConnection.GenerateClientId();
                nativeConnection.ResetCommandNumber();
            }
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="AbstractCommand.Execute()" />
        public override ServerResponse Execute()
        {
            Log.Trace(nameof(ConnectCommand) + "::" + nameof(Execute));

            if (Connection.Connected)
            {
                Log.Error
                    (
                        nameof(ConnectCommand) + "::" + nameof(Execute) + ": "
                        + Resources.ConnectCommand_Execute_AlreadyConnected
                    );

                throw new IrbisException(Resources.IrbisConnection_AlreadyConnected);
            }

            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.RegisterClient;

            string username = Username ?? Connection.Username;
            if (string.IsNullOrEmpty(username))
            {
                Log.Error
                    (
                        nameof(ConnectCommand) + "::" + nameof(Execute) + ": "
                        + Resources.ConnectCommand_UsernameNotSpecified
                    );

                throw new IrbisException(Resources.ConnectCommand_UsernameNotSpecified);
            }

            string password = Password ?? Connection.Password;
            if (string.IsNullOrEmpty(password))
            {
                Log.Error
                    (
                        nameof(ConnectCommand) + "::" + nameof(Execute) + ": "
                        + Resources.ConnectCommand_PasswordNotSpecified
                    );

                throw new IrbisException(Resources.ConnectCommand_PasswordNotSpecified);
            }

            query.UserLogin = username;
            query.UserPassword = password;

            query.Arguments.Add(username);
            query.Arguments.Add(password);

            ServerResponse result;

            while (true)
            {
                result = Execute(query);

                Log.Trace
                    (
                        nameof(ConnectCommand) + "::" + nameof(Execute)
                        + ": returnCode="
                        + result.ReturnCode
                    );

                // CLIENT_ALREADY_EXISTS
                if (result.ReturnCode == -3337)
                {
                    IrbisConnection connection = Connection as IrbisConnection;
                    int newId = ReferenceEquals(connection, null)
                        ? Connection.ClientID + 1
                        : connection.GenerateClientId();
                    query.ClientID = newId;
                }
                else
                {
                    break;
                }
            }

            if (result.ReturnCode == 0)
            {
                ConfirmationInterval = result.RequireInt32();
                Configuration = result.RemainingAnsiText();
            }

            ServerVersion = result.ServerVersion;

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="AbstractCommand.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ConnectCommand> verifier
                = new Verifier<ConnectCommand>(this, throwOnError);

            verifier
                .NotNullNorEmpty(Username, nameof(Username))
                .NotNullNorEmpty(Password, nameof(Password));

            return verifier.Result;
        }

        #endregion
    }
}
