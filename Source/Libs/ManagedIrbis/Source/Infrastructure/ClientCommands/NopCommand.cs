// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NopCommand.cs -- unlock the database
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
    /// Unlock the database on the server.
    /// </summary>
    [PublicAPI]
    public class NopCommand
        : ClientCommand
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public NopCommand
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
            query.CommandCode = CommandCode.Nop;

            ServerResponse result = Execute(query);
            result.GetReturnCode();

            return result;
        }

        #endregion
    }
}
