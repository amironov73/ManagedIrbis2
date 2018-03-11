﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PrintTableCommand.cs -- build table on the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM.Logging;

using CodeJam;

using JetBrains.Annotations;

using MoonSharp.Interpreter;

#endregion

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// Build table on the server.
    /// </summary>
    [PublicAPI]
    [MoonSharpUserData]
    public sealed class PrintTableCommand
        : AbstractCommand
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrintTableCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region AbstractCommand members

        /// <summary>
        /// Check the server response.
        /// </summary>
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            Code.NotNull(response, "response");

            // Ignore the result
            response.RefuseAnReturnCode();
        }

        /// <summary>
        /// Create client query.
        /// </summary>
        public override ClientQuery CreateQuery()
        {
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

            ClientQuery result = base.CreateQuery();
            result.CommandCode = CommandCode.Print;

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

            result
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

            return result;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        public override ServerResponse Execute
            (
                ClientQuery query
            )
        {
            Code.NotNull(query, "query");

            ServerResponse result = base.Execute(query);

            Result = "{\\rtf1 "
                + result.RemainingUtfText()
                + "}";

            return result;
        }

        #endregion
    }
}
