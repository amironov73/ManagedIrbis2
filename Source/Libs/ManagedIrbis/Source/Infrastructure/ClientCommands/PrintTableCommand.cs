﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PrintTableCommand.cs -- build table on the server
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
    /// Build table on the server.
    /// </summary>
    [PublicAPI]
    public sealed class PrintTableCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Table definition.
        /// </summary>
        [CanBeNull]
        public TableDefinition Definition { get; set; }

        /// <summary>
        /// Result of the command execution.
        /// </summary>
        [CanBeNull]
        public string Result { get; set; }

        #endregion

        #region Construction

        ///// <summary>
        ///// Constructor.
        ///// </summary>
        //public PrintTableCommand
        //    (
        //        [NotNull] IIrbisConnection connection
        //    )
        //    : base(connection)
        //{
        //}

        #endregion

        #region ClientCommand members

        ///// <summary>
        ///// Check the server response.
        ///// </summary>
        //public override void CheckResponse
        //    (
        //        ServerResponse response
        //    )
        //{
        //    Sure.NotNull(response, nameof(response));

        //    // Ignore the result
        //    response.RefuseAnReturnCode();
        //}

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            IIrbisConnection connection = context.Connection;

            TableDefinition definition = Definition;

            if (ReferenceEquals(definition, null))
            {
                Log.Error
                    (
                        "PrintTableCommand::CreateQuery: "
                        + "Definition is null"
                    );

                throw new IrbisException("Definition == null");
            }

            ClientQuery query = CreateQuery(connection, CommandCode.Print);

            // "7"         PRINT
            // "IBIS"      database
            // "@tabf1w"   table
            // ""          headers
            // ""          mod
            // "T=A$"      search
            // "0"         min
            // "0"         max
            // ""          sequential
            // ""          mfn list

            query
                .AddAnsi(definition.DatabaseName)
                .AddAnsi(definition.Table)
                .Add(string.Empty) // instead of headers
                .Add(definition.Mode)
                .AddUtf8(definition.SearchQuery)
                .Add(definition.MinMfn)
                .Add(definition.MaxMfn)
                .AddUtf8(definition.SequentialQuery)
                .Add(string.Empty) // instead of MFN list
                ;

            ServerResponse result = base.Execute(connection, query);

            Result = "{\\rtf1 "
                + result.RemainingUtfText()
                + "}";

            return result;
        }

        #endregion
    }
}
