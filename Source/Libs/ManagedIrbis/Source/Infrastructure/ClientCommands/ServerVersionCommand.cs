// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ServerVersionCommand.cs --
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
    ///
    /// </summary>
    [PublicAPI]
    public class ServerVersionCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Result.
        /// </summary>
        [CanBeNull]
        public IrbisVersion Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerVersionCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="ClientCommand.Execute()" />
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.ServerInfo;

            ServerResponse response = Execute(query);
            response.GetReturnCode();
            Result = IrbisVersion.ParseServerResponse(response);

            return response;
        }

        #endregion
    }
}
