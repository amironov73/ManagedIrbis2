// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReloadDictionaryCommand.cs --
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
    /// Reload database dictionary.
    /// </summary>
    [PublicAPI]
    public sealed class ReloadDictionaryCommand
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
        public ReloadDictionaryCommand
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
            query.CommandCode = CommandCode.ReloadDictionary;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "ReloadDictionaryCommand::CreateQuery: "
                    + "database not specified"
                );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            ServerResponse result = Execute(query);
            result.GetReturnCode();

            return result;
        }

        /// <inheritdoc cref="ClientCommand.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<ReloadDictionaryCommand> verifier
                = new Verifier<ReloadDictionaryCommand>(this, throwOnError);

            verifier
                .NotNullNorEmpty(Database, nameof(Database));

            return verifier.Result;
        }

        #endregion
    }
}
