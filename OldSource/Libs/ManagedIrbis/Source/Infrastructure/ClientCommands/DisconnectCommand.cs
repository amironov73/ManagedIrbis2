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
        public override void Execute
            (
                ClientContext context
            )
        {
            Log.Trace(nameof(DisconnectCommand) + "::" + nameof(Execute));

            ClientQuery query = CreateQuery(context, CommandCode.UnregisterClient);
            string userName = context.GetUsername(null, relax: true);
            if (!string.IsNullOrEmpty(userName))
            {
                query.AddAnsi(userName);
            }

            ServerResponse response = BaseExecute(context);

            Log.Trace
                (
                    nameof(DisconnectCommand) + "::" + nameof(Execute)
                    + ": returnCode="
                    + response.ReturnCode.ToInvariantString()
                );
        }

        #endregion
    }
}
