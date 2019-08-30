// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NonCloseableTextReader.cs -- non-closeable TextReader
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;

using JetBrains.Annotations;

#endregion

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace AM.IO
{
    /// <summary>
    /// Non-closeable <see cref="T:System.IO.TextReader"/>.
    /// Call <see cref="RealClose"/> to close it.
    /// </summary>
    [PublicAPI]
    public class NonCloseableTextReader
        : TextReader,
          IDisposable
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public NonCloseableTextReader
            (
                TextReader innerReader
            )
        {
            Sure.NotNull(innerReader, nameof(innerReader));

            _innerReader = innerReader;
        }

        #endregion

        #region Private members

        private readonly TextReader _innerReader;

        #endregion

        #region Public methods

        /// <summary>
        /// Really closes the reader.
        /// </summary>
        public virtual void RealClose()
        {
            //_innerReader.Dispose();
            _innerReader.Close();
        }

        #endregion

        #region TextReader members

        /// <inheritdoc cref="TextReader.Close"/>
        public override void Close()
        {
            // Nothing to do actually
        }

        /// <inheritdoc cref="TextReader.Dispose(bool)"/>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        protected override void Dispose
            (
                bool disposing
            )
        {
            // Nothing to do actually
        }

        /// <inheritdoc cref="TextReader.Peek" />
        [DebuggerStepThrough]
        public override int Peek()
        {
            return _innerReader.Peek();
        }

        /// <inheritdoc cref="TextReader.Read()" />
        [DebuggerStepThrough]
        public override int Read()
        {
            return _innerReader.Read();
        }

        /// <inheritdoc cref="TextReader.Read(char[],int,int)" />
        [DebuggerStepThrough]
        public override int Read
            (
                char[] buffer,
                int index,
                int count
            )
        {
            return _innerReader.Read(buffer, index, count);
        }

        /// <inheritdoc cref="TextReader.ReadBlock(char[],int,int)" />
        [DebuggerStepThrough]
        public override int ReadBlock
            (
                char[] buffer,
                int index,
                int count
            )
        {
            return _innerReader.ReadBlock(buffer, index, count);
        }

        /// <inheritdoc cref="TextReader.ReadLine" />
        public override string? ReadLine()
        {
            return _innerReader.ReadLine();
        }

        /// <inheritdoc cref="TextReader.ReadToEnd" />
        [DebuggerStepThrough]
        public override string ReadToEnd()
        {
            return _innerReader.ReadToEnd();
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose"/>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        void IDisposable.Dispose()
        {
            // Nothing to do actually
        }

        #endregion
    }
}
