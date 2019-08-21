﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisText.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Text.RegularExpressions;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class IrbisText
    {
        #region Constants

        /// <summary>
        /// Irbis line delimiter.
        /// </summary>
        public const string IrbisDelimiter = "\x001F\x001E";

        /// <summary>
        /// Standard Windows line delimiter.
        /// </summary>
        public const string StandardDelimiter = "\r\n";

        /// <summary>
        /// Standard Windows line delimiter.
        /// </summary>
        public const string WindowsDelimiter = "\r\n";

        #endregion

        #region Private members

        private static char[] _delimiters = { '\x1F' };

        private static string _CleanupEvaluator
            (
                [NotNull] Match match
            )
        {
            int length = match.Value.Length;

            if ((length & 1) == 0)
            {
                return new string('.', length / 2);
            }

            return new string('.', length / 2 + 2);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Cleanup the text.
        /// </summary>
        public static string? CleanupMarkup
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text)
                || !text.Contains("[["))
            {
                return text;
            }

            while (true)
            {
                // Remove repeating area delimiters.
                string result = Regex.Replace
                    (
                        text,
                        @"\[\[(?<tag>.*?)\]\](?<meat>.*?)\[\[/\k<tag>\]\]",
                        "${meat}"
                    );
                if (result == text)
                {
                    text = result;
                    break;
                }

                text = result;
            }


            return text;
        }

        /// <summary>
        /// Cleanup the text.
        /// </summary>
        [CanBeNull]
        public static string CleanupText
            (
                [CanBeNull] string text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // Remove repeating area delimiters.
            string result = Regex.Replace
                (
                    text,
                    @"(\.\s-\s){2,}",
                    ". - "
                );

            // Cleanup repeating dots
            result = Regex.Replace
                (
                    result,
                    @"\.{2,}",
                    _CleanupEvaluator
                );

            // Remove the area delimiters at the paragraph end.
            result = Regex.Replace
                (
                    result,
                    @"(\.\s-\s)+(<br>|<br\s*/>|\\par|\x0A|\x0D\x0A)",
                    "$2"
                );

            return result;
        }

        /// <summary>
        /// Convert IRBIS line endings to standard.
        /// </summary>
        public static string? IrbisToWindows
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!text.Contains(IrbisDelimiter))
            {
                return text;
            }

            string result = text.Replace
                (
                    IrbisDelimiter,
                    WindowsDelimiter
                );

            return result;
        }

        /// <summary>
        /// Split IRBIS-delimited text to lines.
        /// </summary>
        [NotNull]
        public static string[] SplitIrbisToLines
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return StringUtility.EmptyArray;
            }

            var provenText = IrbisToWindows(text)!;
            string[] result = string.IsNullOrEmpty(provenText)
                ? new[] { string.Empty }
                : provenText.Split
                    (
                        _delimiters,
                        StringSplitOptions.None
                    );

            return result;
        }

        /// <summary>
        /// Convert text to lower case.
        /// </summary>
        public static string? ToLower
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            string result = text.ToLowerInvariant();

            return result;
        }

        /// <summary>
        /// Convert text to upper case.
        /// </summary>
        public static string? ToUpper
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            // TODO use isisucw.txt ?

            string result = text.ToUpperInvariant();

            return result;
        }

        /// <summary>
        /// Convert standard line endings to IRBIS.
        /// </summary>
        public static string? WindowsToIrbis
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!text.Contains(WindowsDelimiter))
            {
                return text;
            }

            string result = text.Replace
                (
                    WindowsDelimiter,
                    IrbisDelimiter
                );

            return result;
        }

        #endregion
    }
}
