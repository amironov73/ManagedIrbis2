// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* EncodingUtility.cs -- text encoding related routines
 *  Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace AM.Text
{
    /// <summary>
    /// Text encoding related routines.
    /// </summary>
    [PublicAPI]
    public static class EncodingUtility
    {
        #region Properties

        /// <summary>
        /// Default encoding.
        /// </summary>
        /// <remarks>
        /// Reduce if/else preprocessing.
        /// </remarks>
        [NotNull]
        public static Encoding DefaultEncoding
        {
            get
            {
                return Encoding.GetEncoding(0);
            }
        }

        private static Encoding _windows1251;

        /// <summary>
        /// Gets the Windows-1251 (cyrillic) <see cref="Encoding"/>.
        /// </summary>
        [NotNull]
        public static Encoding Windows1251
        {
            [DebuggerStepThrough]
            get
            {
                if (ReferenceEquals(_windows1251, null))
                {
                    _windows1251 = Encoding.GetEncoding(1251);
                }

                return _windows1251;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Change encoding of the text.
        /// </summary>
        [CanBeNull]
        public static string ChangeEncoding
            (
                [CanBeNull] string text,
                [NotNull] Encoding fromEncoding,
                [NotNull] Encoding toEncoding
            )
        {
            Sure.NotNull(fromEncoding, nameof(fromEncoding));
            Sure.NotNull(toEncoding, nameof(toEncoding));

            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] bytes = toEncoding.GetBytes(text);
            string result = GetString
                (
                    fromEncoding,
                    bytes
                );

            return result;
        }

        /// <summary>
        /// Get string from bytes.
        /// </summary>
        /// <remarks>
        /// Reduce if/else preprocessing.
        /// </remarks>
        [NotNull]
        public static string GetString
            (
                [NotNull] Encoding encoding,
                [NotNull] byte[] bytes
            )
        {
            Sure.NotNull(encoding, "encoding");
            Sure.NotNull(bytes, "bytes");

            string result = encoding.GetString(bytes);

            return result;
        }

        #endregion
    }
}
