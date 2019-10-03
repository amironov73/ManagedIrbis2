// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextReaderUtility.cs -- helpers for TextReader
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Helpers for <see cref="TextReader"/>.
    /// </summary>
    [PublicAPI]
    public static class TextReaderUtility
    {
        #region Public methods

        /// <summary>
        /// Open file for reading.
        /// </summary>
        public static StreamReader OpenRead
            (
                string fileName,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            var result = new StreamReader
                (
                    File.OpenRead(fileName),
                    encoding
                );

            return result;
        }

        /// <summary>
        /// Обязательное чтение строки.
        /// </summary>
        public static string RequireLine
            (
                this TextReader reader
            )
        {
            var result = reader.ReadLine();
            if (ReferenceEquals(result, null))
            {
                Log.Error
                    (
                        nameof(TextReaderUtility)
                        + "::"
                        + nameof(RequireLine)
                        + ": "
                        + "unexpected end of stream"
                    );

                throw new ArsMagnaException
                    (
                        "Unexpected end of stream"
                    );
            }

            return result;
        }

        #endregion
    }
}
