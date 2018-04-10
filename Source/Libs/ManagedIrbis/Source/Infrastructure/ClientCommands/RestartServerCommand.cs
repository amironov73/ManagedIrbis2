// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RestartServerCommand.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Restart the IRBIS64 server (save and restore the server state).
    /// </summary>
    [PublicAPI]
    public sealed class RestartServerCommand
        : ClientCommand
    {
        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            CreateQuery(context, CommandCode.RestartServer);
            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
