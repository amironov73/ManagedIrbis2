﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Globalization;
using System.IO;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class IrbisUtility
    {
        #region Public methods

        /// <summary>
        ///
        /// </summary>
        [NotNull]
        [MustUseReturnValue]
        public static string EncodePercentString
            (
                [CanBeNull] byte[] array
            )
        {
            if (ReferenceEquals(array, null)
                || array.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder(array.Length * 3);
            foreach (byte b in array)
            {
                if (b >= 'A' && b <= 'Z'
                    || b >= 'a' && b <= 'z'
                    || b >= '0' && b <= '9'
                    )
                {
                    result.Append((char)b);
                }
                else
                {
                    result.AppendFormat("%{0:X2}", b);
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///
        /// </summary>
        [NotNull]
        [MustUseReturnValue]
        public static byte[] DecodePercentString
            (
                [CanBeNull] string text
            )
        {
            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return Array.Empty<byte>();
            }

            int predictedLength = text.Length / 2;
            using (MemoryStream stream = new MemoryStream(predictedLength))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c != '%')
                    {
                        stream.WriteByte((byte) c);
                    }
                    else
                    {
                        if (i >= text.Length - 2)
                        {
                            Log.Error
                                (
                                    nameof(IrbisUtility) + "::" + nameof(DecodePercentString)
                                    + ": unexpected end of stream"
                                );

                            throw new FormatException(nameof(text));
                        }

                        byte b = byte.Parse
                            (
                                text.Substring(i + 1, 2),
                                NumberStyles.HexNumber
                            );
                        stream.WriteByte(b);
                        i += 2;
                    }
                }

                return stream.ToArray();
            }
        }

        #endregion
    }
}
