// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Utility.cs -- bunch of useful routines.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using AM.Logging;

using JetBrains.Annotations;

#endregion

// ReSharper disable InvokeAsExtensionMethod

namespace AM
{
    /// <summary>
    /// Bunch of useful routines.
    /// </summary>
    [PublicAPI]
    public static class Utility
    {
        #region Public methods

        /// <summary>
        /// Throw <see cref="ArgumentNullException"/>
        /// if given value is <c>null</c>.
        /// </summary>
        [NotNull]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>
            (
                [CanBeNull] this T value
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
        [NotNull]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T1 ThrowIfNull<T1, T2>
        (
            [CanBeNull] this T1 value
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
        [NotNull]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ThrowIfNull<T>
        (
            [CanBeNull] this T value,
            [NotNull] string message
        )
            where T : class
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
        /// Преобразование любого значения в строку.
        /// </summary>
        /// <returns>Для <c>null</c> возвращается "(null)".
        /// </returns>
        [Pure]
        [NotNull]
        public static string ToVisibleString<T>
            (
                [CanBeNull] this T value
            )
        {
            if (ReferenceEquals(value, null))
            {
                return "(null)";
            }

            string result = value.ToString();

            return StringUtility.ToVisibleString(result);
        }

        #endregion
    }
}
