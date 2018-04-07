// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RestartServerCommand.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Reload database dictionary.
    /// </summary>
    [PublicAPI]
    public sealed class RestartServerCommand
        : ClientCommand
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public RestartServerCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.RestartServer;

            ServerResponse result = Execute(query);
            result.GetReturnCode();

            return result;
        }

        #endregion
    }
}
