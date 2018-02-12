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
        /// Gets the CP866 (cyrillic) <see cref="Encoding"/>.
        /// </summary>
        [NotNull]
        public static Encoding Cp866
        {
            [DebuggerStepThrough]
            get
            {
                if (ReferenceEquals(_cp866, null))
                {
                    RegisterRequiredProviders();
                    _cp866 = Encoding.GetEncoding(866);
                }

                return _cp866;
            }
        }

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
                    RegisterRequiredProviders();
                    _windows1251 = Encoding.GetEncoding(1251);
                }

                return _windows1251;
            }
        }

        #endregion

        #region Private members

        private static Encoding _cp866, _windows1251;

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
            string result = fromEncoding.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Register required encoding providers.
        /// </summary>
        public static void RegisterRequiredProviders()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #endregion
    }
}
