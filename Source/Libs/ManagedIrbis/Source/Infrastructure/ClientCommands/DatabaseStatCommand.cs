// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* DatabaseStatCommand.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Linq;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Database stat.
    /// </summary>
    [PublicAPI]
    public sealed class DatabaseStatCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Client query definition.
        /// </summary>
        public StatDefinition Definition { get; set; }

        /// <summary>
        /// Result of the command.
        /// </summary>
        public string Result { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public DatabaseStatCommand
            (
                [NotNull] IIrbisConnection connection
            )
            : base(connection)
        {
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion

        #region AbstractCommand members

        /// <inheritdoc cref="ClientCommand.CheckResponse" />
        public override void CheckResponse
            (
                ServerResponse response
            )
        {
            Sure.NotNull(response, nameof(response));

            // Ignore the result
            response.RefuseAnReturnCode();
        }

        /// <inheritdoc cref="ClientCommand.Execute()" />
        public override ServerResponse Execute()
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode.DatabaseStat;

            // "2"               STAT
            // "IBIS"            database
            // "v200^a,10,100,1" field
            // "T=A$"            search
            // "0"               min
            // "0"               max
            // ""                sequential
            // ""                mfn list

            string items = string.Join
            (
                IrbisText.IrbisDelimiter,
                Definition.Items
                    .Select(item => item.ToString())
                    .ToArray()
            );

            query
                .Add(Definition.DatabaseName)
                .Add(items)
                .AddUtf8(Definition.SearchQuery)
                .Add(Definition.MinMfn)
                .Add(Definition.MaxMfn)
                .AddUtf8(Definition.SequentialQuery)
                .Add(string.Empty) // instead of MFN list
                ;

            ServerResponse result = Execute(query);

            Result = "{\\rtf1 "
                + result.RemainingUtfText()
                + "}";

            return result;
        }

        #endregion
    }
}
