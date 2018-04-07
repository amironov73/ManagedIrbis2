// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DatabaseInfoCommand.cs --
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
    ///
    /// </summary>
    [PublicAPI]
    public class DatabaseInfoCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Result.
        /// </summary>
        [CanBeNull]
        public DatabaseInfo Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseInfoCommand
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
            query.CommandCode = CommandCode.RecordList;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "DatabaseInfoCommand::CreateQuery: "
                    + "database not specified"
                );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            ServerResponse response = base.Execute(query);
            Result = DatabaseInfo.ParseServerResponse(response);
            Result.Name = Database;

            return response;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="ClientCommand.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<DatabaseInfoCommand> verifier
                = new Verifier<DatabaseInfoCommand>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, "Database");

            return verifier.Result;
        }

        #endregion
    }
}
