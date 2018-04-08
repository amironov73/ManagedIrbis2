// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReloadMasterFileCommand.cs --
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

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public ReloadMasterFileCommand
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

            ClientQuery query = CreateQuery(connection);
            query.CommandCode = CommandCode.ReloadMasterFile;

            string database = Database ?? connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                    (
                        "ReloadMasteFileCommand::CreateQuery: "
                        + "database not specified"
                    );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            ServerResponse result = Execute(connection, query);
            result.GetReturnCode();

            return result;
        }

        /// <inheritdoc cref="ClientCommand.Verify" />
        public override bool Verify(bool throwOnError)
        {
            Verifier<ReloadMasterFileCommand> verifier
                = new Verifier<ReloadMasterFileCommand>(this, throwOnError);

            verifier
                .NotNullNorEmpty(Database, "Database");

            return verifier.Result;
        }

        #endregion
    }
}
