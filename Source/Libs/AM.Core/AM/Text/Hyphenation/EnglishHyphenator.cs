// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* EnglishHyphenator.cs -- simple hyphenator for English language
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using AM.Logging;

using JetBrains.Annotations;

#endregion

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo


namespace AM.Text.Hyphenation
{
    /// <summary>
    /// Simple <see cref="Hyphenator"/> for English language.
    /// </summary>
    [PublicAPI]
    public class EnglishHyphenator
        : Hyphenator
    {
        #region Private members

        private static readonly char[] _vowels =
            {
                'a', 'e', 'i', 'o', 'u', 'y'
            };

        private static readonly char[] _consonants =
            {
                'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
                'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
            };

        private static readonly string[] _prefixes =
            {
                "dis", "per", "pre", "sub"
            };

        private static bool _IsVowel(string str, int index)
        {
            return Array.IndexOf(_vowels, str[index]) >= 0;
        }

        private static bool _IsConsonant(string str, int index)
        {
            return Array.IndexOf(_consonants, str[index]) >= 0;
        }

        #endregion

        #region Hyphenator members

        /// <inheritdoc cref="Hyphenator.LanguageName" />
        public override string LanguageName => "English";

        /// <inheritdoc cref="Hyphenator.RecognizeWord"/>
        [ExcludeFromCodeCoverage]
        public override bool RecognizeWord
            (
                string theWord
            )
        {
            Log.Error
                (
                    nameof(EnglishHyphenator)
                    + "::"
                    + nameof(RecognizeWord)
                    + ": not implemented"
                );

            throw new NotImplementedException();
        }

        /// <inheritdoc cref="Hyphenator.Hyphenate" />
        public override int[] Hyphenate
            (
                string word
            )
        {
            if (string.IsNullOrEmpty(word)
                 || word.Length < 4)
            {
                return Array.Empty<int>();
            }

            // Нельзя переносить слова, содержащие прописные буквы
            // (кроме первой, разумеется).
            for (var i = 1; i < word.Length; i++)
            {
                if (char.IsUpper(word, i))
                {
                    return Array.Empty<int>();
                }
            }

            var result = new List<int>();
            var len = word.Length - 2;
            // Можно переносить сразу за гласной
            for (var i = 1; i < len; i++)
            {
                if (_IsVowel(word, i))
                {
                    // Если после гласной много согласных,
                    // переносим по согласным
                    if (i + 1 < len
                         && _IsConsonant(word, i + 1)
                         && _IsConsonant(word, i + 2)
                         && _IsConsonant(word, i + 3))
                    {
                        result.Add(++i);
                        i++;
                    }
                    else
                    {
                        result.Add(i);
                    }
                }
            }
            // Можно переносить между двумя согласными
            for (var i = 1; i < len; i++)
            {
                if (_IsConsonant(word, i)
                     && _IsConsonant(word, i + 1))
                {
                    result.Add(i);
                }
            }
            result.Sort();
            // Отдаем предпочтение переносу по удвоенной согласной
            for (var i = 0; i < result.Count; )
            {
                var pos = result[i];
                if (_IsConsonant(word, pos + 1)
                     && word[pos + 1] == word[pos + 2])
                {
                    result.RemoveAt(i);
                    continue;
                }
                if (pos > 2
                    && _IsConsonant(word, pos - 1)
                    && word[pos - 1] == word[pos - 2])
                {
                    result.RemoveAt(i);
                    continue;
                }
                i++;
            }
            // Нельзя переносить после двух согласных подряд
            for (var i = 0; i < result.Count; )
            {
                var pos = result[i];
                if (pos > 2 && _IsConsonant(word, pos)
                     && _IsConsonant(word, pos - 1))
                {
                    result.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            result.Sort();
            // Нельзя разрывать приставку
            if (result.Count > 0)
            {
                foreach (var prefix in _prefixes)
                {
                    if (word.StartsWith(prefix))
                    {
                        result[0] = prefix.Length - 1;
                        break;
                    }
                }
            }
            // Нельзя переносить часть слова, состоящую только
            // из согласных
            if (result.Count > 0)
            {
                var last = result[result.Count - 1];
                var canBreak = false;
                for (var i = last + 1; i < word.Length; i++)
                {
                    if (_IsVowel(word, i))
                    {
                        canBreak = true;
                    }
                }
                if (!canBreak)
                {
                    result.Remove(last);
                }
            }

            return result
                .Distinct()
                .OrderBy(_ => _)
                .ToArray();
        }

        #endregion
    }
}
