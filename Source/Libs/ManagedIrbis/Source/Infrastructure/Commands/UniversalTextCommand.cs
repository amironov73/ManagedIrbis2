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

namespace ManagedIrbis.Infrastructure.Commands
{
    /// <summary>
    /// Universal command with text lines.
    /// </summary>
    [PublicAPI]
    public sealed class UniversalTextCommand
        : AbstractCommand
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

        #region Public methods

        #endregion

        #region AbstractCommand members

        /// <summary>
        /// Create client query.
        /// </summary>
        public override ClientQuery CreateQuery()
        {
            ClientQuery result = base.CreateQuery();
            result.CommandCode = CommandCode;

            foreach (TextWithEncoding line in TextLines)
            {
                result.Add(line);
            }

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
