// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReloadMasterFileCommand.cs --
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
    /// Reload database master file.
    /// </summary>
    [PublicAPI]
    public sealed class ReloadMasterFileCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReloadMasterFileCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReloadMasterFileCommand
            (
                [CanBeNull] string database
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));

            Database = database;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.ReloadMasterFile);
            query.AddAnsi(context.GetDatabase(Database));
            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
