// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FastNumber.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Fast routines for integer numbers.
    /// </summary>
    [PublicAPI]
    public static class FastNumber
    {
        #region Public methods

        // ==========================================================

        /// <summary>
        /// Convert integer to string.
        /// </summary>
        public static string Int32ToString
            (
                int number
            )
        {
            var buffer = new char[10];
            var offset = 9;
            if (number == 0)
            {
                buffer[offset] = '0';
                offset--;
            }
            else
            {
                for (; number != 0; offset--)
                {
                    number = Math.DivRem(number, 10, out int rem);
                    buffer[offset] = (char) ('0' + rem);
                }
            }

            return new string(buffer, offset + 1, 9 - offset);
        }

        // ==========================================================

        /// <summary>
        /// Convert integer to string.
        /// </summary>
        public static string Int64ToString
            (
                long number
            )
        {
            var buffer = new char[20];
            var offset = 19;
            if (number == 0)
            {
                buffer[offset] = '0';
                offset--;
            }
            else
            {
                for (; number != 0; offset--)
                {
                    number = Math.DivRem(number, 10, out long rem);
                    buffer[offset] = (char) ('0' + rem);
                }
            }

            return new string(buffer, offset + 1, 19 - offset);
        }

        // ==========================================================

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                string text
            )
        {
            var result = 0;
            unchecked
            {
                foreach (char c in text)
                {
                    result = result * 10 + c - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                string text,
                int offset,
                int length
            )
        {
            var result = 0;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                char[] text,
                int offset,
                int length
            )
        {
            var result = 0;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                byte[] text,
                int offset,
                int length
            )
        {
            var result = 0;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                ReadOnlyMemory<char> text
            )
        {
            var result = 0;
            var span = text.Span;
            unchecked
            {
                for (int i = 0; i < text.Length; i++)
                {
                    result = result * 10 + span[i] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static int ParseInt32
            (
                ReadOnlyMemory<byte> text
            )
        {
            var result = 0;
            var span = text.Span;
            unchecked
            {
                for (int i = 0; i < text.Length; i++)
                {
                    result = result * 10 + span[i] - '0';
                }
            }

            return result;
        }

        // ==========================================================

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                string text
            )
        {
            var result = 0L;
            unchecked
            {
                foreach (char c in text)
                {
                    result = result * 10 + c - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                string text,
                int offset,
                int length
            )
        {
            var result = 0L;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                char[] text,
                int offset,
                int length
            )
        {
            var result = 0L;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                byte[] text,
                int offset,
                int length
            )
        {
            var result = 0L;
            unchecked
            {
                for (; length > 0; length--, offset++)
                {
                    result = result * 10 + text[offset] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                ReadOnlyMemory<char> text
            )
        {
            var result = 0L;
            var span = text.Span;
            unchecked
            {
                for (int i = 0; i < text.Length; i++)
                {
                    result = result * 10 + span[i] - '0';
                }
            }

            return result;
        }

        /// <summary>
        /// Fast number parsing.
        /// </summary>
        public static long ParseInt64
            (
                ReadOnlyMemory<byte> text
            )
        {
            var result = 0L;
            var span = text.Span;
            unchecked
            {
                for (int i = 0; i < text.Length; i++)
                {
                    result = result * 10 + span[i] - '0';
                }
            }

            return result;
        }

        // ==========================================================

        #endregion
    }
}
