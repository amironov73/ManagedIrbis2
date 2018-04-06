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

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// Reload database dictionary.
    /// </summary>
    [PublicAPI]
    public sealed class RestartServerCommand
        : AbstractCommand
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

        #region AbstractCommand members

        /// <inheritdoc cref="AbstractCommand.Execute()" />
        public override ServerResponse Execute()
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
