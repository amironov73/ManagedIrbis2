﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Sequence.cs -- 
 * Ars Magna project, http://arsmagna.ru 
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Inspired by LINQ Sequence.cs.
    /// </summary>
    [PublicAPI]
    public static class Sequence
    {
        #region Public methods

        /// <summary>
        /// First or given item of sequence.
        /// </summary>
        [CanBeNull]
        public static T FirstOr<T>
            (
                [NotNull] this IEnumerable<T> list,
                [CanBeNull] T defaultValue
            )
        {
            Sure.NotNull(list, nameof(list));

            foreach (T item in list)
            {
                return item;
            }

            return defaultValue;
        }

        /// <summary>
        /// Sequence of one element.
        /// </summary>
        [Pure]
        [NotNull]
        public static IEnumerable<T> FromItem<T>
            (
                T item
            )
        {
            yield return item;
        }

        /// <summary>
        /// Sequence of two items.
        /// </summary>
        [Pure]
        [NotNull]
        public static IEnumerable<T> FromItems<T>
            (
                T item1,
                T item2
            )
        {
            yield return item1;
            yield return item2;
        }

        /// <summary>
        /// Sequence of three items.
        /// </summary>
        [Pure]
        [NotNull]
        public static IEnumerable<T> FromItems<T>
            (
                T item1,
                T item2,
                T item3
            )
        {
            yield return item1;
            yield return item2;
            yield return item3;
        }

        /// <summary>
        /// Make sequence of given items.
        /// </summary>
        [Pure]
        [NotNull]
        public static IEnumerable<T> FromItems<T>
            (
                params T[] items
            )
        {
            foreach (T item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Get max or default value for the sequence.
        /// </summary>
        public static T MaxOrDefault<T>
            (
                [NotNull] this IEnumerable<T> sequence,
                T defaultValue
            )
        {
            Sure.NotNull(sequence, nameof(sequence));

            T[] array = sequence.ToArray();
            if (array.Length == 0)
            {
                return defaultValue;
            }

            T result = array.Max();

            return result;
        }

        /// <summary>
        /// Get max or default value for the sequence.
        /// </summary>
        public static TOutput MaxOrDefault<TInput,TOutput>
            (
                [NotNull] this IEnumerable<TInput> sequence,
                [NotNull] Func<TInput,TOutput> selector,
                TOutput defaultValue
            )
        {
            Sure.NotNull(sequence, nameof(sequence));
            Sure.NotNull(selector, nameof(selector));

            TInput[] array = sequence.ToArray();
            if (array.Length == 0)
            {
                return defaultValue;
            }

            TOutput result = array.Max(selector);

            return result;
        }

        /// <summary>
        /// Отбирает из последовательности только
        /// ненулевые элементы.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> NonNullItems<T>
            (
                [NotNull] this IEnumerable<T> sequence
            )
            where T : class
        {
            Sure.NotNull(sequence, nameof(sequence));

            return sequence.Where(value => !ReferenceEquals(value, null));
        }

        /// <summary>
        /// Отбирает из последовательности только непустые строки.
        /// </summary>
        public static IEnumerable<string> NonEmptyLines
            (
                this IEnumerable<string> sequence
            )
        {
            return sequence.Where ( line => !string.IsNullOrEmpty(line) );
        }

        /// <summary>
        /// Repeats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns>Sequence of specified values.</returns>
        [Pure]
        [NotNull]
        public static IEnumerable<T> Repeat<T>
            (
                [CanBeNull] T value,
                int count
            )
        {
            while (count-- > 0)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Repeats the specified list.
        /// </summary>
        [NotNull]
        public static IEnumerable<T> Repeat<T>
            (
                [NotNull] IEnumerable<T> list,
                int count
            )
        {
            Sure.NotNull(list, nameof(list));

            while (count-- > 0)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (T value in list)
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Replaces items in the specified list.
        /// </summary>
        /// <param name="list">The list to process to.</param>
        /// <param name="replaceFrom">Item to replace from.</param>
        /// <param name="replaceTo">Replacement.</param>
        /// <returns>List with replaced items.</returns>
        [NotNull]
        public static IEnumerable<T> Replace<T>
            (
                [NotNull] this IEnumerable<T> list,
                [CanBeNull] T replaceFrom,
                [CanBeNull] T replaceTo
            )
            where T : IEquatable<T>
        {
            Sure.NotNull(list, nameof(list));

            foreach (T item in list)
            {
                if (item.Equals(replaceFrom))
                {
                    yield return replaceTo;
                }
                else
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Extracts segment from the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Segment.</returns>
        [NotNull]
        public static IEnumerable<T> Segment<T>
            (
                [NotNull] this IEnumerable<T> list,
                int offset,
                int count
            )
        {
            Sure.NotNull(list, nameof(list));
            Sure.NonNegative(offset, nameof(offset));
            Sure.NonNegative(count, nameof(count));

            int index = 0;
            foreach (T obj in list)
            {
                if (index < offset)
                {
                    index++;
                }
                else if (count > 0)
                {
                    yield return obj;
                    count--;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Добавляем некоторое действие к каждому
        /// элементу последовательности.
        /// </summary>
        [NotNull]
        public static IEnumerable<T> Tee<T>
            (
                [NotNull] this IEnumerable<T> list,
                [NotNull] Action<T> action
            )
        {
            Sure.NotNull(list, nameof(list));
            Sure.NotNull(action, nameof(action));

            foreach (T item in list)
            {
                action(item);

                yield return item;
            }
        }

        /// <summary>
        /// Добавляем некоторое действие к каждому
        /// элементу последовательности.
        /// </summary>
        [NotNull]
        public static IEnumerable<T> Tee<T>
            (
                [NotNull] this IEnumerable<T> list,
                [NotNull] Action<int, T> action
            )
        {
            Sure.NotNull(list, nameof(list));
            Sure.NotNull(action, nameof(action));

            int index = 0;
            foreach (T item in list)
            {
                action(index, item);
                index++;

                yield return item;
            }
        }

        /// <summary>
        /// Separate the sequence with given separator.
        /// </summary>
        public static IEnumerable Separate
            (
                [NotNull] this IEnumerable sequence,
                [CanBeNull] object separator
            )
        {
            Sure.NotNull(sequence, nameof(sequence));

            bool first = true;
            foreach (object obj in sequence)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return separator;
                }
                yield return obj;
            }
        }

        /// <summary>
        /// Slice the sequence to pieces
        /// with given size.
        /// </summary>
        [NotNull]
        public static IEnumerable<T[]> Slice<T>
            (
                [NotNull] this IEnumerable<T> sequence,
                int pieceSize
            )
        {
            Sure.NotNull(sequence, nameof(sequence));
            if (pieceSize <= 0)
            {
                Log.Error
                    (
                        nameof(Sequence) + "::" + nameof(Slice)
                        + "pieceSize="
                        + pieceSize
                    );

                throw new ArgumentOutOfRangeException(nameof(pieceSize));
            }

            List<T> piece = new List<T>(pieceSize);
            foreach (T item in sequence)
            {
                piece.Add(item);
                if (piece.Count >= pieceSize)
                {
                    yield return piece.ToArray();
                    piece = new List<T>(pieceSize);
                }
            }

            if (piece.Count != 0)
            {
                yield return piece.ToArray();
            }
        }

        #endregion
    }
}
