// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DisconnectCommand.cs -- disconnect from the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
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
        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            Log.Trace("DisconnectCommand::Execute");

            ClientQuery query = base.CreateQuery(connection, CommandCode.UnregisterClient);

            query.AddAnsi(connection.Username);

            ServerResponse result = Execute(connection, query);

            Log.Trace
                (
                    nameof(DisconnectCommand) + "::" + nameof(Execute)
                    + ": returnCode="
                    + result.ReturnCode.ToInvariantString()
                );

            if (connection is IrbisConnection iconnection)
            {
                iconnection._connected = false;
            }

            return result;
        }

        #endregion
    }
}
