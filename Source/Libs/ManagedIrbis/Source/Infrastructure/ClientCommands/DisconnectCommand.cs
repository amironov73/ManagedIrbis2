// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DisconnectCommand.cs -- disconnect from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Disconnect from the server.
    /// </summary>
    [PublicAPI]
    public class DisconnectCommand
        : ClientCommand
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public DisconnectCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
            Log.Trace("DisconnectCommand::Constructor");
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            Log.Trace("DisconnectCommand::Execute");

            ClientQuery query = base.CreateQuery();
            query.CommandCode = CommandCode.UnregisterClient;

            query.AddAnsi(Connection.Username);

            ServerResponse result = Execute(query);

            Log.Trace
                (
                    "DisconnectCommand::Execute: returnCode="
                    + result.ReturnCode
                );

            IrbisConnection connection = Connection as IrbisConnection;
            if (!ReferenceEquals(connection, null))
            {
                connection._connected = false;
            }

            return result;
        }

        /// <inheritdoc cref="ClientCommand.CheckResponse" />
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            // Ignore the result
        }

        #endregion
    }
}
