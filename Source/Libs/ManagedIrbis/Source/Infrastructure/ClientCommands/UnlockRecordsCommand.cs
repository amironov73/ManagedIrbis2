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

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Truncate the database on the server.
    /// </summary>
    [PublicAPI]
    public class UnlockRecordsCommand
        : ClientCommand
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
        public UnlockRecordsCommand()
        {
            Records = new List<int>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UnlockRecordsCommand
            (
                [NotNull] string database,
                [NotNull] IEnumerable<int> records
            )
        {
            Sure.NotNullNorEmpty(database, nameof(database));
            Sure.NotNull(records, nameof(records));

            Database = database;
            Records = new List<int>(records);
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery(context, CommandCode.UnlockRecords);
            query.AddAnsi(context.GetDatabase(Database));

            if (Records.Count == 0)
            {
                Log.Error
                    (
                        nameof(UnlockRecordsCommand) + "::" + nameof(Execute)
                        + ": " + Resources.UnlockRecordsCommand_RecordListIsEmpty
                    );

                throw new IrbisException(Resources.UnlockRecordsCommand_RecordListIsEmpty);
            }
            query.Arguments.AddRange(Records.Cast<object>());

            ServerResponse response = BaseExecute(context);
            CheckResponse(response);
        }

        #endregion
    }
}
