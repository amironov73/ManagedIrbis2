// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PrintTableCommand.cs -- build table on the server
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

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

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrintTableCommand()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrintTableCommand
            (
                [NotNull] TableDefinition definition
            )
        {
            Sure.NotNull(definition, nameof(definition));

            Definition = definition;
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override void Execute
            (
                ClientContext context
            )
        {
            TableDefinition definition = Definition.ThrowIfNull(nameof(Definition));
            ClientQuery query = CreateQuery(context, CommandCode.Print);

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
                .AddAnsi(definition.DatabaseName.ThrowIfNull(nameof(definition.DatabaseName)))
                .AddAnsi(definition.Table.ThrowIfNull(nameof(definition.Table)))
                .Add(string.Empty) // instead of headers
                .Add(definition.Mode)
                .AddUtf8(definition.SearchQuery)
                .Add(definition.MinMfn)
                .Add(definition.MaxMfn)
                .AddUtf8(definition.SequentialQuery)
                .Add(string.Empty) // instead of MFN list
                ;

            ServerResponse response = BaseExecute(context);

            Result = "{\\rtf1 "
                + response.RemainingUtfText()
                + "}";
        }

        #endregion
    }
}
