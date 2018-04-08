﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CreateDictionaryCommand.cs -- create database index
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Create database index on the server.
    /// </summary>
    [PublicAPI]
    public class CreateDictionaryCommand
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
        //public CreateDictionaryCommand
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
            query.CommandCode = CommandCode.CreateDictionary;

            string database = Database ?? connection.Database;
            if (string.IsNullOrEmpty(database))
            {
                Log.Error
                    (
                        "CreateDictionaryCommand::CreateQuery: "
                        + Resources.IrbisNetworkUtility_DatabaseNotSpecified
                    );

                throw new IrbisException(Resources.IrbisNetworkUtility_DatabaseNotSpecified);
            }
            query.AddAnsi(database);

            ServerResponse result = Execute(connection, query);

            return result;
        }

        #endregion
    }
}
