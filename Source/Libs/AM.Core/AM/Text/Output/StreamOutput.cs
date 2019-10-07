// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FileOutput.cs -- файловый вывод
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.Text.Output
{
    /// <summary>
    /// Output to stream.
    /// </summary>
    [PublicAPI]
    public sealed class StreamOutput
        : AbstractOutput
    {
        #region Properties

        /// <summary>
        /// Inner writer.
        /// </summary>
        [NotNull]
        public TextWriter Writer { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public StreamOutput
            (
                [NotNull] TextWriter writer
            )
        {
            Sure.NotNull(writer, nameof(writer));

            Writer = writer;
            _ownWriter = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StreamOutput
            (
                [NotNull] Stream stream,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(stream, nameof(stream));
            Sure.NotNull(encoding, nameof(encoding));

            Writer = new StreamWriter(stream, encoding);
            _ownWriter = true;
        }

        #endregion

        #region Private members

        private readonly bool _ownWriter;

        #endregion

        #region AbstractOutput members

        /// <summary>
        /// Флаг: был ли вывод с помощью WriteError.
        /// </summary>
        public override bool HaveError { get; set; }

        /// <summary>
        /// Очищает вывод, например, окно.
        /// Надо переопределить в потомке.
        /// </summary>
        public override AbstractOutput Clear()
        {
            HaveError = false;

            return this;
        }

        /// <summary>
        /// Конфигурирование объекта.
        /// Надо переопределить в потомке.
        /// </summary>
        public override AbstractOutput Configure
            (
                string configuration
            )
        {
            Log.Error
                (
                    "StreamOutput::Configure: "
                    + "not implemented"
                );

            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод, который нужно переопределить
        /// в потомке.
        /// </summary>
        public override AbstractOutput Write
            (
                string text
            )
        {
            Writer.Write(text);

            return this;
        }

        /// <summary>
        /// Выводит ошибку. Например, красным цветом.
        /// Надо переопределить в потомке.
        /// </summary>
        public override AbstractOutput WriteError
            (
                string text
            )
        {
            Writer.Write(text);
            HaveError = true;

            return this;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public override void Dispose()
        {
            if (_ownWriter)
            {
                Writer.Dispose();
            }

            base.Dispose();
        }

        #endregion
    }
}
