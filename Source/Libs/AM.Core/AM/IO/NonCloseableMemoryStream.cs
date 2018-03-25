// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NonCloseableMemoryStream.cs -- stream that likes to be non-closed.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Non-closeable version of a <see cref="MemoryStream"/>.
    /// </summary>
    [PublicAPI]
    public sealed class NonCloseableMemoryStream
        : MemoryStream
    {
        #region Construction

        /// <inheritdoc />
        public NonCloseableMemoryStream()
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(byte[] buffer) : base(buffer)
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(byte[] buffer, bool writable) : base(buffer, writable)
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(byte[] buffer, int index, int count) : base(buffer, index, count)
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(byte[] buffer, int index, int count, bool writable) : base(buffer, index, count, writable)
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) : base(buffer, index, count, writable, publiclyVisible)
        {
        }

        /// <inheritdoc />
        public NonCloseableMemoryStream(int capacity) : base(capacity)
        {
        }

        #endregion

        #region Stream members

        /// <inheritdoc cref="MemoryStream.Dispose(bool)" />
        protected override void Dispose(bool disposing)
        {
            // Nothing to do here
        }

        /// <inheritdoc cref="Stream.Close" />
        public override void Close()
        {
            // Nothing to do here
        }

        #endregion
    }
}
