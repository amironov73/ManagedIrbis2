// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UniversalTextCommand.cs -- universal command with text lines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Text;

using AM;
using AM.Collections;
using AM.Text;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Infrastructure.ClientCommands
{
    /// <summary>
    /// Universal command with text lines.
    /// </summary>
    [PublicAPI]
    public sealed class UniversalTextCommand
        : ClientCommand
    {
        #region Properties

        /// <summary>
        /// Command code.
        /// </summary>
        [NotNull]
        public string CommandCode { get; }

        /// <summary>
        /// Lines.
        /// </summary>
        [NotNull]
        public NonNullCollection<TextWithEncoding> TextLines { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public UniversalTextCommand
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string commandCode
            )
            : base(connection)
        {
            Sure.NotNull(connection, nameof(connection));
            Sure.NotNullNorEmpty(commandCode, nameof(commandCode));

            CommandCode = commandCode;
            TextLines = new NonNullCollection<TextWithEncoding>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public UniversalTextCommand
            (
                [NotNull] IIrbisConnection connection,
                [NotNull] string commandCode,
                [NotNull] string[] lines,
                [NotNull] Encoding encoding
            )
            : this (connection, commandCode)
        {
            Sure.NotNull(lines, nameof(lines));
            Sure.NotNull(encoding, nameof(encoding));

            foreach (string line in lines)
            {
                TextLines.Add
                    (
                        new TextWithEncoding
                            (
                                line,
                                encoding
                            )
                    );
            }
        }

        #endregion

        #region ClientCommand members

        /// <inheritdoc cref="ClientCommand.Execute(ClientContext)" />
        public override ServerResponse Execute
            (
                ClientContext context
            )
        {
            ClientQuery query = CreateQuery();
            query.CommandCode = CommandCode;

            foreach (TextWithEncoding line in TextLines)
            {
                query.Add(line);
            }

            ServerResponse result = Execute(query);

            return result;
        }

        #endregion

        #region IVerifiable members

        /// <summary>
        /// Verify object state.
        /// </summary>
        public override bool Verify
            (
                bool throwOnError
            )
        {
            Verifier<UniversalTextCommand> verifier
                = new Verifier<UniversalTextCommand>
                    (
                        this,
                        throwOnError
                    );

            verifier
                .NotNullNorEmpty(CommandCode, "CommandCode")
                .NotNull(TextLines, "TextLines")
                .Assert(TextLines.Count > 0, "TextLines.Count")
                .Assert(base.Verify(throwOnError));

            return verifier.Result;
        }

        #endregion
    }
}
