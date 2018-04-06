// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UnlockRecordsCommand.cs -- truncate the database
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Collections.Generic;
using System.Linq;

using AM;
using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// Truncate the database on the server.
    /// </summary>
    [PublicAPI]
    public class UnlockRecordsCommand
        : AbstractCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        /// <summary>
        /// Record list.
        /// </summary>
        [NotNull]
        public List<int> Records { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnlockRecordsCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
            Records = new List<int>();
        }

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="AbstractCommand.Execute()" />
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.UnlockRecords;

            string database = Database ?? Connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                (
                    "UnlockRecordsCommand::CreateQuery: "
                    + "database not specified"
                );

                throw new IrbisException("database not specified");
            }
            query.AddAnsi(database);

            if (Records.Count == 0)
            {
                Log.Error
                (
                    "UnlockRecordsCommand::CreateQuery: "
                    + "record list is empty"
                );

                throw new IrbisException("record list is empty");
            }
            query.Arguments.AddRange(Records.Cast<object>());

            ServerResponse result = Execute(query);
            result.GetReturnCode();

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <inheritdoc cref="IVerifiable.Verify" />
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<UnlockRecordsCommand> verifier
                = new Verifier<UnlockRecordsCommand>
                (
                    this,
                    throwOnError
                );

            verifier
                .NotNullNorEmpty(Database, "Database")
                .Assert(Records.Count != 0, "Records.Count");

            return verifier.Result;
        }

        #endregion
    }
}
