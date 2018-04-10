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

namespace ManagedIrbis.Infrastructure.ClientCommands
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
        : ClientCommand
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

        ///// <summary>
        ///// Doesn't require connection.
        ///// </summary>
        //public override bool RequireConnection
        //{
        //    get { return false; }
        //}

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            Log.Trace(nameof(ConnectCommand) + "::" + nameof(Execute));

            context.CheckAlreadyConnected();

            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(connection, CommandCode.RegisterClient);
            query.UserLogin = context.GetUsername(Username);
            query.UserPassword = context.GetPassword(Password);
            query.Arguments.Add(query.UserLogin);
            query.Arguments.Add(query.UserPassword);

            //while (true)
            //{
                BaseExecute(context);

                //Log.Trace
                //    (
                //        nameof(ConnectCommand) + "::" + nameof(Execute)
                //        + ": returnCode="
                //        + context.Response.ReturnCode
                //    );

                //// CLIENT_ALREADY_EXISTS
                //if (result.ReturnCode == -3337)
                //{
                //    IrbisConnection connection = Connection as IrbisConnection;
                //    int newId = ReferenceEquals(connection, null)
                //        ? Connection.ClientID + 1
                //        : connection.GenerateClientId();
                //    query.ClientID = newId;
                //}
                //else
                //{
                   // break;
                //}
            //}

            //if (result.ReturnCode == 0)
            //{
            //    ConfirmationInterval = result.RequireInt32();
            //    Configuration = result.RemainingAnsiText();
            //}

            //ServerVersion = result.ServerVersion;
        }

        #endregion
    }
}
