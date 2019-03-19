// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Sure.cs -- common assertions
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class Sure
    {
        #region Public methods

        /// <summary>
        /// State assertion
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertState
            (
                [AssertionCondition(AssertionConditionType.IS_FALSE)]
                bool condition,
                [NotNull] string message
            )
        {
            if (!condition)
            {
                throw new InvalidOleVariantTypeException(message);
            }
        }

        /// <summary>
        /// Check whether <paramref name="value"/> is not defined.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Defined<T>
            (
                T value,
                [NotNull, InvokerParameterName] string argumentName
            )
            where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Checks whether specified files exists.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FileExists
            (
                [CanBeNull] string path,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException
                    (
                        argumentName
                        + " : "
                        + path
                    );
            }
        }

        /// <summary>
        /// Checks whether <paramref name="argument"/> fits into
        /// specified range <paramref name="fromValue"/>
        /// to <paramref name="toValue"/>.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InRange
            (
                int argument,
                int fromValue,
                int toValue,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < fromValue
                || argument > toValue)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Checks whether <paramref name="argument"/> fits into
        /// specified range <paramref name="fromValue"/>
        /// to <paramref name="toValue"/>.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InRange
            (
                long argument,
                long fromValue,
                long toValue,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < fromValue
                || argument > toValue)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Checks whether <paramref name="argument"/> fits into
        /// specified range <paramref name="fromValue"/>
        /// to <paramref name="toValue"/>.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InRange
            (
                double argument,
                double fromValue,
                double toValue,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < fromValue
                || argument > toValue)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is not negative.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NonNegative
            (
                int argument,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is not negative.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NonNegative
            (
                long argument,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is not negative.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NonNegative
            (
                double argument,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument < 0.0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Ensures that <paramref name="argument" /> != <c>null</c>.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>
            (
                [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
                [CanBeNull, NoEnumeration] T argument,
                [NotNull, InvokerParameterName] string argumentName
            )
            where T : class
        {
            if (ReferenceEquals(argument, null))
            {
                throw new ArgumentException(argumentName);
            }
        }

        /// <summary>
        /// Ensures that <paramref name="argument" /> != <c>null</c>.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull<T>
            (
                [AssertionCondition(AssertionConditionType.IS_NOT_NULL)]
                [CanBeNull, NoEnumeration] T? argument,
                [NotNull, InvokerParameterName] string argumentName
            )
            where T : struct
        {
            if (ReferenceEquals(argument, null))
            {
                throw new ArgumentException(argumentName);
            }
        }

        /// <summary>
        /// Ensures that <paramref name="argument" />
        /// is not null nor empty.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNullNorEmpty
            (
                [CanBeNull] string argument,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is positive.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive
            (
                int argument,
                [NotNull, InvokerParameterName] string argumentName
            )
        {
            if (argument <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is positive.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive
            (
                long argument,
                [NotNull] string argumentName
            )
        {
            if (argument <= 0.0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Check whether <paramref name="argument"/> is positive.
        /// </summary>
        [DebuggerHidden]
        [AssertionMethod]
        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Positive
            (
                double argument,
                [NotNull] string argumentName
            )
        {
            if (argument <= 0.0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        #endregion
    }
}
