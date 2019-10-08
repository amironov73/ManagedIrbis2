// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Hyphenator.cs -- abstract hyphenator
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Text;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace AM.Text.Hyphenation
{
    /// <summary>
    /// Abstract hyphenator.
    /// </summary>
    [PublicAPI]
    public abstract class Hyphenator
    {
        #region Public methods

        /// <summary>
        /// Gets the language name (e. g. "English" or "Russian").
        /// </summary>
        /// <value>The name of the language.</value>
        public abstract string LanguageName { get; }

        /// <summary>
        /// Determines whether the <see cref="Hyphenator"/>
        /// can split specified word.
        /// </summary>
        /// <param name="theWord">Word to check.</param>
        /// <returns><c>true</c> if word can be processed;
        /// otherwise <c>false</c>.</returns>
        public abstract bool RecognizeWord
            (
                string theWord
            );

        /// <summary>
        /// Hyphenates the word.
        /// </summary>
        /// <param name="word">Word to hyphenate.</param>
        /// <returns>Array of positions where hyphen can be inserted.
        /// </returns>
        public abstract int[] Hyphenate
            (
                string word
            );

        /// <summary>
        /// Show the word in hyphenated form.
        /// </summary>
        /// <param name="word">Word to hyphenate.</param>
        /// <param name="positions">Possible positions of hyphen.
        /// </param>
        /// <returns>Hyphenated word.</returns>
        public static string ShowHyphenated
            (
                string word,
                int[] positions
            )
        {
            Sure.NotNullNorEmpty(word, nameof(word));

            var result = new StringBuilder();
            for (var i = 0; i < word.Length; i++)
            {
                result.Append(word[i]);
                if (Array.IndexOf(positions, i) >= 0)
                {
                    result.Append('-');
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
