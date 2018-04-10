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
            //(
            //    [NotNull] IIrbisConnection connection
            //)
            //: base(connection)
        {
            Records = new List<int>();
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(connection, CommandCode.UnlockRecords);
            query.AddAnsi(context.GetDatabase(Database));

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

            ServerResponse result = BaseExecute(context);
            result.GetReturnCode();
        }

        #endregion
    }
}
