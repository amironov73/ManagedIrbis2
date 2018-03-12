// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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

using CM=System.Configuration.ConfigurationManager;

#endregion

namespace ManagedIrbis
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
        [NotNull]
        public static Encoding Ansi => _ansi;

        /// <summary>
        /// OEM encoding.
        /// </summary>
        [NotNull]
        public static Encoding Oem => _oem;

        /// <summary>
        /// UTF8 encoding.
        /// </summary>
        [NotNull]
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
                    false, // don't emit UTF-8 prefix
                    true   // throw on invalid bytes
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
        [NotNull]
        public static Encoding ByName
            (
                [CanBeNull] string name
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

            Encoding result = Encoding.GetEncoding(name);

            return result;
        }

        /// <summary>
        /// Get encoding from config file.
        /// </summary>
        [NotNull]
        public static Encoding FromConfig
            (
                [NotNull] string key
            )
        {
            Sure.NotNullNorEmpty(key, nameof(key));

            string name = CM.AppSettings[key];
            Encoding result = ByName(name);

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
                    false, // don't emit UTF-8 prefix,
                    false  // don't throw on invalid bytes
                );
        }

        /// <summary>
        /// Override default single-byte encoding.
        /// </summary>
        public static void SetAnsiEncoding
            (
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(encoding, nameof(encoding));

            if (!encoding.IsSingleByte)
            {
                Log.Error
                    (
                        nameof(IrbisEncoding) + "::" + nameof(SetAnsiEncoding)
                        + ": not single-byte encoding"
                    );

                throw new ArgumentOutOfRangeException("encoding");
            }

            _ansi = encoding;
        }

        /// <summary>
        /// Override OEM encoding.
        /// </summary>
        public static void SetOemEncoding
            (
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(encoding, nameof(encoding));

            if (!encoding.IsSingleByte)
            {
                Log.Error
                    (
                        nameof(IrbisEncoding) + "::" + nameof(SetOemEncoding)
                        + ": not single-byte encoding"
                    );

                throw new ArgumentOutOfRangeException("encoding");
            }

            _oem = encoding;
        }

        #endregion
    }
}
