﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TruncateDatabaseCommand.cs -- truncate the database
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
    /// Truncate the database on the server.
    /// </summary>
    [PublicAPI]
    public class TruncateDatabaseCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Database name.
        /// </summary>
        [CanBeNull]
        public string Database { get; set; }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;
            ClientQuery query = CreateQuery(connection, CommandCode.EmptyDatabase);
            query.AddAnsi(context.GetDatabase(Database));

            ServerResponse result = Execute(connection, query);
            CheckResponse(result);

            return result;
        }

        #endregion
    }
}
