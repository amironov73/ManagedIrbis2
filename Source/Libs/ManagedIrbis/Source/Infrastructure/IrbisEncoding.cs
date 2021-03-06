﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisEncoding.cs -- encodings used by IRBIS
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * State: moderate
 */

#region Using directives

using System;
using System.Text;

using AM;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

using CM=System.Configuration.ConfigurationManager;

#endregion

namespace ManagedIrbis.Infrastructure
{
    /// <summary>
    /// Encoding used by IRBIS
    /// </summary>
    [PublicAPI]
    public static class IrbisEncoding
    {
        #region Properties

        /// <summary>
        /// Default single-byte encoding.
        /// </summary>
        public static Encoding Ansi => _ansi;

        /// <summary>
        /// OEM encoding.
        /// </summary>
        public static Encoding Oem => _oem;

        /// <summary>
        /// UTF8 encoding.
        /// </summary>
        public static Encoding Utf8 => _utf8;

        #endregion

        #region Construction

        static IrbisEncoding()
        {
            EncodingUtility.RegisterRequiredProviders();
            _ansi = Encoding.GetEncoding(1251);
            _oem = Encoding.GetEncoding(866);
            _utf8 = new UTF8Encoding
                (
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true
                );
        }

        #endregion

        #region Private members

        private static Encoding _ansi, _oem, _utf8;

        #endregion

        #region Public methods

        /// <summary>
        /// Get encoding by name.
        /// </summary>
        public static Encoding ByName
            (
                string? name
            )
        {
            if (string.IsNullOrEmpty(name))
            {
                return Utf8;
            }

            if (name.SameString("Ansi"))
            {
                return Ansi;
            }

            if (name.SameString("Dos")
                || name.SameString("MsDos")
                || name.SameString("Oem"))
            {
                return Oem;
            }

            if (name.SameString("Utf")
                || name.SameString("Utf8")
                || name.SameString("Utf-8"))
            {
                return Utf8;
            }

            var result = Encoding.GetEncoding(name);

            return result;
        }

        /// <summary>
        /// Get encoding from config file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static Encoding FromConfig
            (
                string key
            )
        {
            Sure.NotNullNorEmpty(key, nameof(key));

            var name = CM.AppSettings[key];
            var result = ByName(name);

            return result;
        }

        /// <summary>
        /// Relax UTF-8 decoder, do not throw exceptions
        /// on invalid bytes.
        /// </summary>
        public static void RelaxUtf8()
        {
            _utf8 = new UTF8Encoding
                (
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: false
                );
        }

        /// <summary>
        /// Strong UTF-8 decoder, throw exceptions
        /// on invalid bytes.
        /// </summary>
        public static void StrongUtf8()
        {
            _utf8 = new UTF8Encoding
                (
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true
                );
        }

        /// <summary>
        /// Override default single-byte encoding.
        /// </summary>
        public static void SetAnsiEncoding
            (
                Encoding encoding
            )
        {
            if (!encoding.IsSingleByte)
            {
                Log.Error
                    (
                        nameof(IrbisEncoding) + "::" + nameof(SetAnsiEncoding)
                        + Resources.IrbisEncoding_NotSingleByteEncoding
                    );

                throw new ArgumentOutOfRangeException(nameof(encoding));
            }

            _ansi = encoding;
        }

        /// <summary>
        /// Override OEM encoding.
        /// </summary>
        public static void SetOemEncoding
            (
                Encoding encoding
            )
        {
            if (!encoding.IsSingleByte)
            {
                Log.Error
                    (
                        nameof(IrbisEncoding) + "::" + nameof(SetOemEncoding)
                        + Resources.IrbisEncoding_NotSingleByteEncoding
                    );

                throw new ArgumentOutOfRangeException(nameof(encoding));
            }

            _oem = encoding;
        }

        #endregion
    }
}
