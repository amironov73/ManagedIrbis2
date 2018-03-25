﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* IrbisFormat.cs -- common format related stuff
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System.Diagnostics;
using System.Text;

using AM;
using AM.Logging;
using AM.Text;

using JetBrains.Annotations;

#endregion

// ReSharper disable InconsistentNaming

namespace ManagedIrbis
{
    /// <summary>
    /// Common format related stuff.
    /// </summary>
    [PublicAPI]
    public static class IrbisFormat
    {
        #region Constants

        /// <summary>
        /// Format ALL.
        /// </summary>
        public const string All = "&uf('+0')";

        /// <summary>
        /// BRIEF format.
        /// </summary>
        public const string Brief = "@brief";

        /// <summary>
        /// IBIS format.
        /// </summary>
        public const string Ibis = "@ibiskw_h";

        /// <summary>
        /// Informational format.
        /// </summary>
        public const string Informational = "@info_w";

        /// <summary>
        /// Optimized format.
        /// </summary>
        public const string Optimized = "@";

        #endregion

        #region Properties

        /// <summary>
        /// Запрещенные символы.
        /// </summary>
        public static char[] ForbiddenCharacters = {'\r', '\n', '\t', '\x1F', '\x1E'};

        #endregion

        #region Public methods

        /// <summary>
        /// Remove comments from the format.
        /// </summary>
        [CanBeNull]
        public static string RemoveComments
            (
                [CanBeNull] string text
            )
        {
            const char ZERO = '\0';

            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            if (!text.Contains("/*"))
            {
                return text;
            }

            StringBuilder result = new StringBuilder(text.Length);
            TextNavigator navigator = new TextNavigator(text);
            char state = ZERO;

            while (!navigator.IsEOF)
            {
                char c = navigator.ReadChar();

                switch (state)
                {
                    case '\'':
                        if (c == '\'')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    case '"':
                        if (c == '"')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    case '|':
                        if (c == '|')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    default:
                        Debug.Assert(state == ZERO, "state == ZERO");

                        if (c == '/')
                        {
                            if (navigator.PeekChar() == '*')
                            {
                                navigator.ReadTo('\r', '\n');
                            }
                            else
                            {
                                result.Append(c);
                            }
                        }
                        else if (c == '\'' || c == '"' || c == '|')
                        {
                            state = c;
                            result.Append(c);
                        }
                        else
                        {
                            result.Append(c);
                        }
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Prepare the dynamic format string.
        /// </summary>
        /// <remarks>Dynamic format string
        /// mustn't contains comments and
        /// string delimiters (no matter
        /// real or IRBIS).
        /// </remarks>
        [CanBeNull]
        public static string PrepareFormat
            (
                [CanBeNull] string text
            )
        {
            const char ZERO = '\0';

            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            text = RemoveComments(text);
            if (!ReferenceEquals(text, null) && text.Length != 0)
            {
                text = text.Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);
            }

            if (ReferenceEquals(text, null) || text.Length == 0)
            {
                return text;
            }

            StringBuilder result = new StringBuilder(text.Length);
            TextNavigator navigator = new TextNavigator(text);

            char state = ZERO;

            // Replace all forbidden characters with spaces
            while (!navigator.IsEOF)
            {
                char c = navigator.ReadChar();

                switch (state)
                {
                    case '\'':
                        if (c == '\'')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    case '"':
                        if (c == '"')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    case '|':
                        if (c == '|')
                        {
                            state = ZERO;
                        }
                        result.Append(c);
                        break;

                    default:
                        Debug.Assert(state == ZERO, "state == ZERO");

                        if (c == '\'' || c == '"' || c == '|')
                        {
                            state = c;
                        }
                        else if (c < ' ')
                        {
                            c = ' ';
                        }
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Verify format string.
        /// </summary>
        public static bool VerifyFormat
            (
                [CanBeNull] string text,
                bool throwOnError
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                Log.Error
                    (
                        nameof(IrbisFormat) + "::" + nameof(VerifyFormat)
                        + ": text is absent"
                    );

                if (throwOnError)
                {
                    throw new VerificationException("text is absent");
                }

                return false;
            }

            // TODO more verification logic

            return true;
        }

        #endregion
    }
}
