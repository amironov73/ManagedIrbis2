// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
#nullable disable
        [CanBeNull]
        public static T FirstOr<T>
            (
                this IEnumerable<T> list,
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
#nullable restore

        /// <summary>
        /// Sequence of one element.
        /// </summary>
        [Pure]
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
#nullable disable
        public static T MaxOrDefault<T>
            (
                this IEnumerable<T> sequence,
                T defaultValue
            )
        {
            var array = sequence.ToArray();
            if (array.Length == 0)
            {
                return defaultValue;
            }

            T result = array.Max();

            return result;
        }
#nullable restore

        /// <summary>
        /// Get max or default value for the sequence.
        /// </summary>
#nullable disable
        public static TOutput MaxOrDefault<TInput,TOutput>
            (
                this IEnumerable<TInput> sequence,
                Func<TInput,TOutput> selector,
                TOutput defaultValue
            )
        {
            var array = sequence.ToArray();
            if (array.Length == 0)
            {
                return defaultValue;
            }

            var result = array.Max(selector);
            return result;
        }
#nullable restore

        /// <summary>
        /// Отбирает из последовательности только
        /// ненулевые элементы.
        /// </summary>
        [ItemNotNull]
        public static IEnumerable<T> NonNullItems<T>
            (
                this IEnumerable<T> sequence
            )
            where T : class
        {
            foreach (var item in sequence)
            {
                if (!ReferenceEquals(item, null))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Отбирает из последовательности только непустые строки.
        /// </summary>
        [ItemNotNull]
        public static IEnumerable<string> NonEmptyLines
            (
                this IEnumerable<string> sequence
            )
        {
            foreach (var line in sequence)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Repeats the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns>Sequence of specified values.</returns>
#nullable disable
        [Pure]
        public static IEnumerable<T> Repeat<T>
            (
                T value,
                int count
            )
        {
            while (count-- > 0)
            {
                yield return value;
            }
        }
#nullable restore

        /// <summary>
        /// Repeats the specified list.
        /// </summary>
        public static IEnumerable<T> Repeat<T>
            (
                IEnumerable<T> list,
                int count
            )
        {
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
#nullable disable
        public static IEnumerable<T> Replace<T>
            (
                this IEnumerable<T> list,
                [CanBeNull] T replaceFrom,
                [CanBeNull] T replaceTo
            )
            where T : IEquatable<T>
        {
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
#nullable restore

        /// <summary>
        /// Extracts segment from the specified list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>Segment.</returns>
        public static IEnumerable<T> Segment<T>
            (
            this IEnumerable<T> list,
                int offset,
                int count
            )
        {
            Sure.NonNegative(offset, nameof(offset));
            Sure.NonNegative(count, nameof(count));

            var index = 0;
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
        public static IEnumerable<T> Tee<T>
            (
                this IEnumerable<T> list,
                Action<T> action
            )
        {
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
        public static IEnumerable<T> Tee<T>
            (
                this IEnumerable<T> list,
                Action<int, T> action
            )
        {
            var index = 0;
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
                this IEnumerable sequence,
                object? separator
            )
        {
            var first = true;
            foreach (var obj in sequence)
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
        public static IEnumerable<T[]> Slice<T>
            (
                this IEnumerable<T> sequence,
                int pieceSize
            )
        {
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

            var piece = new List<T>(pieceSize);
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
