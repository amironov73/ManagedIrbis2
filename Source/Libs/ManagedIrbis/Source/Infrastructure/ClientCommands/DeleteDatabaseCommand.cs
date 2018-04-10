// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DeleteDatabaseCommand.cs -- delete database on the server
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
    /// Delete database on the server.
    /// </summary>
    [PublicAPI]
    public class DeleteDatabaseCommand
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
        public DeleteDatabaseCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteDatabaseCommand
            (
                [NotNull] string database
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
            ClientQuery query = CreateQuery(context, CommandCode.DeleteDatabase);
            query.AddAnsi(context.GetDatabase(Database));
            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
