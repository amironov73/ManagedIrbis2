// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ConsoleUtility.cs -- useful routines for console manipulation
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using AM.IO;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Useful routines for console manipulation.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class ConsoleUtility
    {
        /// <summary>
        /// Перенаправление стандартного вывода в файл.
        /// </summary>
        public static void RedirectStandardOutput
            (
                [NotNull] string fileName,
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNullNorEmpty(fileName, nameof(fileName));
            Sure.NotNull(encoding, nameof(encoding));

            StreamWriter stdOutput = TextWriterUtility.Create
                (
                    fileName,
                    encoding
                );
            stdOutput.AutoFlush = true;

            Console.SetOut(stdOutput);
        }

        /// <summary>
        /// Переключение кодовой страницы вывода консоли.
        /// </summary>
        public static void SetOutputCodePage
            (
                [NotNull] Encoding encoding
            )
        {
            Sure.NotNull(encoding, nameof(encoding));

            StreamWriter stdOutput = new StreamWriter
                (
                    Console.OpenStandardOutput(),
                    encoding
                )
            {
                AutoFlush = true
            };
            Console.SetOut(stdOutput);

            StreamWriter stdError = new StreamWriter
                (
                    Console.OpenStandardError(),
                    encoding
                )
            {
                AutoFlush = true
            };
            Console.SetError(stdError);
        }

        /// <summary>
        /// Переключение кодовой страницы вывода консоли.
        /// </summary>
        public static void SetOutputCodePage
            (
                int codePage
            )
        {
            SetOutputCodePage(Encoding.GetEncoding(codePage));
        }

        /// <summary>
        /// Переключение кодовой страницы вывода консоли.
        /// </summary>
        public static void SetOutputCodePage
            (
                [NotNull] string codePage
            )
        {
            Sure.NotNullNorEmpty(codePage, nameof(codePage));

            SetOutputCodePage(Encoding.GetEncoding(codePage));
        }
    }
}

