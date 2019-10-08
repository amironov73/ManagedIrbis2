// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FileOutput.cs -- файловый вывод
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region

using System;
using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.Text.Output
{
    /// <summary>
    /// Файловый вывод.
    /// </summary>
    [PublicAPI]
    public sealed class FileOutput
        : AbstractOutput
    {
        #region Properties

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string? FileName => _fileName;

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор.
        /// </summary>
        public FileOutput()
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public FileOutput
            (
                string fileName
            )
        {
            Open (fileName);
        }

        #endregion

        #region Private members

        private string? _fileName;

        private TextWriter? _writer;

        #endregion

        #region Public methods

        /// <summary>
        /// Закрытие файла.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Открытие файла.
        /// </summary>
        public void Open
            (
                string fileName,
                bool append = false
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            Close();
            _fileName = fileName;

            FileMode fileMode = append
                ? FileMode.Append
                : FileMode.Create;
            _writer = new StreamWriter
                (
                    File.Open(fileName, fileMode)
                );
        }

        /// <summary>
        /// Открытие файла.
        /// </summary>
        public void Open
            (
                string fileName,
                bool append,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            Close();
            _fileName = fileName;

            FileMode fileMode = append
                ? FileMode.Append
                : FileMode.Create;
            _writer = new StreamWriter
                (
                    File.Open(fileName, fileMode),
                    encoding
                );
        }

        /// <summary>
        /// Открытие файла.
        /// </summary>
        public void Open
            (
                string fileName,
                Encoding encoding
            )
        {
            Open
                (
                    fileName,
                    false,
                    encoding
                );
        }

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
            // TODO: implement properly

            return this;
        }

        /// <summary>
        /// Конфигурирование объекта.
        /// Надо переопределить в потомке.
        /// </summary>
        public override AbstractOutput Configure
            (
                string? configuration
            )
        {
            // TODO: implement properly

            if (!string.IsNullOrEmpty(configuration))
            {
                Open(configuration);
            }

            return this;
        }

        /// <summary>
        /// Метод, который нужно переопределить
        /// в потомке.
        /// </summary>
        public override AbstractOutput Write
            (
                string? text
            )
        {
            if (!ReferenceEquals(_writer, null)
                && !string.IsNullOrEmpty(text))
            {
                _writer.Write(text);
                _writer.Flush();
            }

            return this;
        }

        /// <summary>
        /// Выводит ошибку. Например, красным цветом.
        /// Надо переопределить в потомке.
        /// </summary>
        public override AbstractOutput WriteError
            (
                string? text
            )
        {
            HaveError = true;
            if (!ReferenceEquals(_writer, null))
            {
                _writer.Write(text);
                _writer.Flush();
            }

            return this;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public override void Dispose()
        {
            if (!ReferenceEquals(_writer, null))
            {
                _writer.Dispose();
                _writer = null;
            }
            base.Dispose();
        }

        #endregion
    }
}
