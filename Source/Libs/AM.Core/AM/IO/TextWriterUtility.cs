// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextWriterUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.IO
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class TextWriterUtility
    {
        #region Public methods

        /// <summary>
        /// Open file for append.
        /// </summary>
        public static StreamWriter Append
            (
                string fileName,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            var result = new StreamWriter
                (
                    new FileStream(fileName, FileMode.Append),
                    encoding
                );

            return result;
        }

        /// <summary>
        /// Open file for writing.
        /// </summary>
        public static StreamWriter Create
            (
                string fileName,
                Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));

            var result = new StreamWriter
                (
                    new FileStream(fileName, FileMode.Create),
                    encoding
                );

            return result;
        }

        #endregion
    }
}
