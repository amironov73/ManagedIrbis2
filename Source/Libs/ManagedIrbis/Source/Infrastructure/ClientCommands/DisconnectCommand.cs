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

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public DisconnectCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //    Log.Trace("DisconnectCommand::Constructor");
        //}

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            Log.Trace("DisconnectCommand::Execute");

            ClientQuery query = base.CreateQuery(connection);
            query.CommandCode = CommandCode.UnregisterClient;

            query.AddAnsi(connection.Username);

            ServerResponse result = Execute(connection, query);

            Log.Trace
                (
                    "DisconnectCommand::Execute: returnCode="
                    + result.ReturnCode
                );

            if (connection is IrbisConnection iconnection)
            {
                iconnection._connected = false;
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
