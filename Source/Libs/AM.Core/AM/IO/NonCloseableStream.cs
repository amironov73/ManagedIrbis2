// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NonCloseableStream.cs -- stream that likes to be non-closed.
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
    /// Stream that likes to be non-closed.
    /// To close the stream call
    /// <see cref="M:AM.IO.NonCloseable.NonCloseableStream.ReallyClose"/>.
    /// </summary>
    [PublicAPI]
    public class NonCloseableStream
        : Stream,
          IDisposable
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NonCloseableStream"/> class.
        /// </summary>
        public NonCloseableStream
            (
                [NotNull] Stream innerStream
            )
        {
            Sure.NotNull(innerStream, nameof(innerStream));

            _innerStream = innerStream;
        }

        #endregion

        #region Private members

        private readonly Stream _innerStream;

        #endregion

        #region Public methods

        /// <summary>
        /// Really closes the stream.
        /// </summary>
        public virtual void ReallyClose()
        {
            _innerStream.Dispose();
        }

        #endregion

        #region Stream members

        /// <inheritdoc cref="Stream.CanRead" />
        public override bool CanRead
        {
            [DebuggerStepThrough]
            get
            {
                return _innerStream.CanRead;
            }
        }

        /// <inheritdoc cref="Stream.CanSeek" />
        public override bool CanSeek
        {
            [DebuggerStepThrough]
            get
            {
                return _innerStream.CanSeek;
            }
        }

        /// <inheritdoc cref="Stream.CanWrite" />
        public override bool CanWrite
        {
            [DebuggerStepThrough]
            get
            {
                return _innerStream.CanWrite;
            }
        }

        /// <summary>
        /// NOT closes the current stream and releases any resources
        /// (such as sockets and file handles) associated with the current stream.
        /// </summary>
        /// <seealso cref="M:AM.IO.NonCloseable.NonCloseableStream.ReallyClose"/>
        [DebuggerStepThrough]
        public override void Close()
        {
            // Nothing to do actually
        }

        /// <inheritdoc cref="Stream.Flush" />
        [DebuggerStepThrough]
        public override void Flush()
        {
            _innerStream.Flush();
        }

        /// <inheritdoc cref="Stream.Length" />
        public override long Length
        {
            [DebuggerStepThrough]
            get
            {
                return _innerStream.Length;
            }
        }

        /// <inheritdoc cref="Stream.Position" />
        public override long Position
        {
            [DebuggerStepThrough]
            get
            {
                return _innerStream.Position;
            }
            [DebuggerStepThrough]
            set
            {
                _innerStream.Position = value;
            }
        }

        /// <inheritdoc cref="Stream.Read" />
        [DebuggerStepThrough]
        public override int Read
            (
                byte[] buffer,
                int offset,
                int count
            )
        {
            return _innerStream.Read(buffer, offset, count);
        }

        /// <inheritdoc cref="Stream.Seek" />
        [DebuggerStepThrough]
        public override long Seek
            (
                long offset,
                SeekOrigin origin
            )
        {
            return _innerStream.Seek(offset, origin);
        }

        /// <inheritdoc cref="Stream.SetLength" />
        [DebuggerStepThrough]
        public override void SetLength
            (
                long value
            )
        {
            _innerStream.SetLength(value);
        }

        /// <inheritdoc cref="Stream.Write" />
        [DebuggerStepThrough]
        public override void Write
            (
                byte[] buffer,
                int offset,
                int count
            )
        {
            _innerStream.Write(buffer, offset, count);
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        void IDisposable.Dispose()
        {
            // Nothing to do actually
        }

        #endregion
    }
}
