// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UnlockDatabaseCommand.cs -- unlock the database
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
    /// Unlock the database on the server.
    /// </summary>
    [PublicAPI]
    public class UnlockDatabaseCommand
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

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public UnlockDatabaseCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //}

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = base.CreateQuery(connection);
            query.CommandCode = CommandCode.UnlockDatabase;

            string database = Database ?? connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                    (
                        "UnlockDatabaseCommand::CreateQuery: "
                        + "database not specified"
                    );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            ServerResponse result = base.Execute(connection, query);
            result.GetReturnCode();

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="ClientCommand.Verify"/>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<UnlockDatabaseCommand> verifier
                = new Verifier<UnlockDatabaseCommand>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, nameof(Database));

            return verifier.Result;
        }

        #endregion
    }
}
