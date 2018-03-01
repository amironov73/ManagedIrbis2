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

        private static char[] _vowels =
            {
                'a', 'e', 'i', 'o', 'u', 'y'
            };

        private static char[] _consonants =
            {
                'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm',
                'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'
            };

        private static string[] _prefixes =
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
                    nameof(EnglishHyphenator) + "::" + nameof(RecognizeWord)
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
            Sure.NotNull(word, nameof(word));

            if (string.IsNullOrEmpty(word)
                 || word.Length < 4)
            {
                return EmptyArray<int>.Value;
            }

            // Нельзя переносить слова, содержащие прописные буквы
            // (кроме первой, разумеется).
            for (int i = 1; i < word.Length; i++)
            {
                if (char.IsUpper(word, i))
                {
                    return EmptyArray<int>.Value;
                }
            }

            List<int> result = new List<int>();
            int len = word.Length - 2;
            // Можно переносить сразу за гласной
            for (int i = 1; i < len; i++)
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
            for (int i = 1; i < len; i++)
            {
                if (_IsConsonant(word, i)
                     && _IsConsonant(word, i + 1))
                {
                    result.Add(i);
                }
            }
            result.Sort();
            // Отдаем предпочтение переносу по удвоенной согласной
            for (int i = 0; i < result.Count; )
            {
                int pos = result[i];
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
            for (int i = 0; i < result.Count; )
            {
                int pos = result[i];
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
                foreach (string prefix in _prefixes)
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
                int last = result[result.Count - 1];
                bool canBreak = false;
                for (int i = last + 1; i < word.Length; i++)
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
