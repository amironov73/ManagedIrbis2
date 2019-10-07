﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* OutputWriter.cs -- wrapper for AbstractOutput
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.Text.Output
{
    /// <summary>
    /// Wrapper for <see cref="AbstractOutput"/>.
    /// </summary>
    [PublicAPI]
    public sealed class OutputWriter
        : TextWriter
    {
        #region Properties

        /// <summary>
        /// Inner <see cref="AbstractOutput"/>.
        /// </summary>
        [NotNull]
        public AbstractOutput Output { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public OutputWriter
            (
                [NotNull] AbstractOutput output
            )
        {
            Sure.NotNull(output, nameof(output));

            Output = output;
        }

        #endregion

        #region TextWriter members

        /// <summary>
        /// When overridden in a derived class,
        /// returns the character encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding => Encoding.Default;

        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        public override void WriteLine()
        {
            Output.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string? value)
        {
            Output.Write(value);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If <paramref name="value" /> is null, only the line terminator is written.</param>
        public override void WriteLine(string value)
        {
            Output.WriteLine(value);
        }

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            Write(new string(value, 1));
        }

        /// <summary>
        /// Writes a character array to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public override void Write(char[] buffer)
        {
            Write(new string(buffer));
        }

        /// <summary>
        /// Writes a subarray of characters to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="TextWriter.Dispose(bool)"/>
        protected override void Dispose
            (
                bool disposing
            )
        {
            Output.Dispose();
        }

        #endregion
    }
}
