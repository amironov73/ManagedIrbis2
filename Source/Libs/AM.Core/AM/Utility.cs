// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Utility.cs -- bunch of useful routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Bunch of useful routines.
    /// </summary>
    [PublicAPI]
    public static class Utility
    {
        #region Public methods

        // =========================================================

        /// <summary>
        /// Hexadecimal dump of the byte array.
        /// </summary>
        [Pure]
        public static string DumpBytes
            (
                byte[] buffer
            )
        {
            var result = new StringBuilder(buffer.Length * 5);

            int offset;

            for (offset = 0; offset < buffer.Length; offset += 16)
            {
                result.AppendFormat
                    (
                        "{0:X6}:",
                        offset
                    );

                var run = Math.Min(buffer.Length - offset, 16);

                for (var i = 0; i < run; i++)
                {
                    result.AppendFormat
                        (
                            " {0:X2}",
                            buffer[offset + i]
                        );
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        // =========================================================

        /// <summary>
        /// Compare two sequences.
        /// </summary>
        /// <remarks>Borrowed from StackOverflow:
        /// http://stackoverflow.com/questions/1680602/what-is-the-algorithm-used-by-the-memberwise-equality-test-in-net-structs
        /// </remarks>
        [Pure]
        public static bool EnumerableEquals
            (
                IEnumerable? left,
                IEnumerable? right
            )
        {
            if (ReferenceEquals(left, null)
                || ReferenceEquals(right, null))
            {
                return false;
            }

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            var rightEnumerator = right.GetEnumerator();
            rightEnumerator.Reset();

            foreach (var leftItem in left)
            {
                // unequal amount of items
                if (!rightEnumerator.MoveNext())
                {
                    return false;
                }

                if (ReferenceEquals(leftItem, null))
                {
                    return false;
                }

                var rightItem = rightEnumerator.Current;
                if (ReferenceEquals(rightItem, null))
                {
                    return false;
                }

                if (!MemberwiseEquals
                    (
                        leftItem,
                        rightItem
                    ))
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================

        /// <summary>
        /// Aggregate hashcode for some objects.
        /// </summary>
        /// <remarks>Borrowed from Tom DuPont:
        /// http://www.tomdupont.net/2014/02/how-to-combine-hashcodes.html
        /// </remarks>
        [Pure]
        public static int GetHashCodeAggregate<T>
            (
                this IEnumerable<T> source
            )
        {
            return GetHashCodeAggregate(source, 17);
        }

        /// <summary>
        /// Aggregate hashcode for some objects.
        /// </summary>
        /// <remarks>Borrowed from Tom DuPont:
        /// http://www.tomdupont.net/2014/02/how-to-combine-hashcodes.html
        /// </remarks>
        [Pure]
        public static int GetHashCodeAggregate<T>
            (
                this IEnumerable<T> source,
                int hash
            )
        {
            unchecked
            {
                foreach (var item in source)
                {
                    if (!ReferenceEquals(item, null))
                    {
                        hash = hash * 31 + item.GetHashCode();
                    }
                }
            }

            return hash;
        }

        // =========================================================

        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
#nullable disable
        [Pure]
        public static T GetItem<T>
            (
                this T[] array,
                int index,
                T defaultValue
            )
        {
            Sure.NotNull(array, nameof(array));

            index = index >= 0
                ? index
                : array.Length + index;

            var result = index >= 0 && index < array.Length
                ? array[index]
                : defaultValue;

            return result;
        }
#nullable restore

        /// <summary>
        /// Get item from the array by specified index.
        /// </summary>
        [Pure]
        public static T? GetItem<T>
            (
                this T[] array,
                int index
            )
            where T: class
        {
            return GetItem(array, index, default);
        }

        /// <summary>
        /// Get item from the list by specified index.
        /// </summary>
        [Pure]
        public static T? GetItem<T>
            (
                this IList<T> list,
                int index,
                T? defaultValue
            )
            where T: class
        {
            Sure.NotNull(list, nameof(list));

            index = index >= 0
                ? index
                : list.Count + index;

            var result = index >= 0 && index < list.Count
                ? list[index]
                : defaultValue;

            return result;
        }

        /// <summary>
        /// Get item from the list by specified index.
        /// </summary>
        public static T? GetItem<T>
            (
                this IList<T> list,
                int index
            )
            where T: class
        {
            return GetItem(list, index, default);
        }

        // =========================================================

        /// <summary>
        /// Determines whether is one of the specified values.
        /// </summary>
        [Pure]
        public static bool IsOneOf<T>
            (
                T value,
                params T[] array
            )
            where T : IComparable<T>
        {
            foreach (var one in array)
            {
                if (value.CompareTo(one) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        // =========================================================

        /// <summary>
        /// Implementation of a memberwise comparison
        /// for objects.
        /// </summary>
        /// <remarks>Borrowed from StackOverflow:
        /// http://stackoverflow.com/questions/1680602/what-is-the-algorithm-used-by-the-memberwise-equality-test-in-net-structs
        /// </remarks>
        [Pure]
        public static bool MemberwiseEquals
            (
                object? left,
                object? right
            )
        {
            if (ReferenceEquals(left, null)
                || ReferenceEquals(right, null))
            {
                return false;
            }

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            var type = left.GetType();
            if (type != right.GetType())
            {
                return false;
            }

            if (type.IsValueType)
            {
                return left.Equals(right);
            }

            var equals = type.GetMethod("Equals");
            if (ReferenceEquals(equals, null))
            {
                return false;
            }
            if (type == equals.DeclaringType)
            {
                return left.Equals(right);
            }

            var leftEnumerable = left as IEnumerable;
            var rightEnumerable = right as IEnumerable;
            if (!ReferenceEquals(leftEnumerable, null))
            {
                return EnumerableEquals
                    (
                        leftEnumerable,
                        rightEnumerable
                    );
            }

            // compare each property
            foreach (var info in type.GetProperties
                (
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.GetProperty
                ))
            {
                // TODO: need to special-case indexable properties
                if (!MemberwiseEquals
                    (
                        info.GetValue(left, null),
                        info.GetValue(right, null)
                    ))
                {
                    return false;
                }
            }

            // compare each field
            foreach (var info in type.GetFields
                (
                    BindingFlags.GetField
                    | BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Instance
                ))
            {
                if (!MemberwiseEquals
                    (
                        info.GetValue(left),
                        info.GetValue(right))
                )
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================

        /// <summary>
        /// Compares two object by public instance properties.
        /// </summary>
        /// <remarks>Borrowed from StackOverflow:
        /// http://stackoverflow.com/questions/506096/comparing-object-properties-in-c-sharp
        /// </remarks>
        [Pure]
        public static bool PropertyEquals
            (
                [CanBeNull] object left,
                [CanBeNull] object right
            )
        {
            if (ReferenceEquals(left, null)
                || ReferenceEquals(right, null))
            {
                return false;
            }

            if (ReferenceEquals(left, right))
            {
                return true;
            }

            var type = left.GetType();
            if (type != right.GetType())
            {
                return false;
            }

            var properties = type.GetProperties
                (
                    BindingFlags.Public
                    | BindingFlags.Instance
                );

            foreach (var property in properties)
            {
                var leftValue = property.GetValue(left, null);
                var rightValue = property.GetValue(right, null);

                if (ReferenceEquals(leftValue, null)
                    || ReferenceEquals(rightValue, null))
                {
                    return false;
                }

                if (!leftValue.Equals(rightValue))
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================

        /// <summary>
        /// Safe access value, when the value is null,
        /// does not throw an exception.
        /// </summary>
        public static T SafeValue<T>
            (
                this T? value
            )
            where T : struct
        {
            return value ?? default(T);
        }

        // =========================================================

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>
            (
                this T? value
            )
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                Log.Error
                    (
                        nameof(Utility) + "::" + nameof(ThrowIfNull)
                    );

                throw new ArgumentException(nameof(value));
            }

            return value;
        }

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T1 ThrowIfNull<T1, T2>
            (
                this T1? value
            )
            where T1 : class
            where T2 : Exception, new()
        {
            if (ReferenceEquals(value, null))
            {
                Log.Error
                    (
                        nameof(Utility) + "::" + nameof(ThrowIfNull)
                    );

                throw new T2();
            }

            return value;
        }

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>
            (
                this T? value,
                string message
            )
            where T: class
        {
            Sure.NotNull(message, nameof(message));

            if (ReferenceEquals(value, null))
            {
                Log.Error
                    (
                        nameof(Utility) + "::" + nameof(ThrowIfNull)
                        + ": "
                        + message
                    );

                throw new ArgumentException(message);
            }

            return value;
        }

        /// <summary>
        /// Convert any value to visible string.
        /// </summary>
        /// <returns>Для <c>null</c> возвращается "(null)".
        /// </returns>
#nullable disable
        [Pure]
        public static string ToVisibleString<T>
            (
                this T value
            )
        {
            if (ReferenceEquals(value, null))
            {
                return "(null)";
            }

            var result1 = value.ToString();

            // ReSharper disable InvokeAsExtensionMethod
            var result2 = StringUtility.ToVisibleString(result1);
            // ReSharper restore InvokeAsExtensionMethod

            return string.IsNullOrEmpty(result2)
                ? "(null)"
                : result2;
        }
#nullable restore

        #endregion
    }
}
