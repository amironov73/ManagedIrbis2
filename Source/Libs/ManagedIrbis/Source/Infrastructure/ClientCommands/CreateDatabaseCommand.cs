// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CreateDatabaseCommand.cs -- create new database on the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 * TODO use Template property
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Create new database on the server.
    /// </summary>
    /// <remarks>For Administrator only.</remarks>
    [PublicAPI]
    public class CreateDatabaseCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Name for new database.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Description of the database.
        /// </summary>
        [CanBeNull]
        public string Description { get; set; }

        /// <summary>
        /// Will the database be visible to reader?
        /// </summary>
        public bool ReaderAccess { get; set; }

        /// <summary>
        /// Template database name.
        /// </summary>
        [CanBeNull]
        public string Template { get; set; }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.CreateDatabase);

            // Layout is:
            // NEWDB          // database name
            // New database   // description
            // 0              // reader access

            query
                .Add(context.GetDatabase(Database))
                .Add(Description)
                .Add(ReaderAccess);

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);

            // Response is (ANSI):
            // 0
            // NewDB NEWDB,New database,0 - Создана новая БД NEWDB
            // CloseDB -
            // Exit C:\IRBIS64_2015\workdir\1126_0.ibf
        }

        #endregion
    }
}
