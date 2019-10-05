// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ActionUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

// ReSharper disable UseNullPropagation

namespace AM
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class ActionUtility
    {
        #region Public methods

        /// <summary>
        /// Call the <paramref name="action"/> if not <c>null</c>.
        /// </summary>
        public static void SafeCall
            (
                this Action? action
            )
        {
            if (!ReferenceEquals(action, null))
            {
                action();
            }
        }

        /// <summary>
        /// Call the <paramref name="action"/> if not <c>null</c>.
        /// </summary>
        public static void SafeCall<T>
            (
                this Action<T>? action,
                T argument1
            )
        {
            if (!ReferenceEquals(action, null))
            {
                action(argument1);
            }
        }

        /// <summary>
        /// Call the <paramref name="action"/> if not <c>null</c>.
        /// </summary>
        public static void SafeCall<T1, T2>
            (
                this Action<T1, T2>? action,
                T1 argument1,
                T2 argument2
            )
        {
            if (!ReferenceEquals(action, null))
            {
                action(argument1, argument2);
            }
        }

        /// <summary>
        /// Call the <paramref name="action"/> if not <c>null</c>.
        /// </summary>
        public static void SafeCall<T1, T2, T3>
            (
                this Action<T1, T2, T3>? action,
                T1 argument1,
                T2 argument2,
                T3 argument3
            )
        {
            if (!ReferenceEquals(action, null))
            {
                action(argument1, argument2, argument3);
            }
        }

        /// <summary>
        /// Call the <paramref name="function"/> if not <c>null</c>.
        /// </summary>
        public static TResult SafeCall<TResult>
            (
                this Func<TResult>? function,
                TResult defaultResult = default
            )
        {
            var result = ReferenceEquals(function, null)
                ? defaultResult
                : function();

            return result;
        }

        /// <summary>
        /// Call the <paramref name="function"/> if not <c>null</c>.
        /// </summary>
        public static TResult SafeCall<T1, TResult>
            (
                this Func<T1, TResult>? function,
                T1 argument1,
                TResult defaultResult = default
            )
        {
            var result = ReferenceEquals(function, null)
                ? defaultResult
                : function(argument1);

            return result;
        }

        /// <summary>
        /// Call the <paramref name="function"/> if not <c>null</c>.
        /// </summary>
        public static TResult SafeCall<T1, T2, TResult>
            (
                this Func<T1, T2, TResult>? function,
                T1 argument1,
                T2 argument2,
                TResult defaultResult = default
            )
        {
            var result = ReferenceEquals(function, null)
                ? defaultResult
                : function(argument1, argument2);

            return result;
        }

        /// <summary>
        /// Call the <paramref name="function"/> if not <c>null</c>.
        /// </summary>
        public static TResult SafeCall<T1, T2, T3, TResult>
            (
                this Func<T1, T2, T3, TResult>? function,
                T1 argument1,
                T2 argument2,
                T3 argument3,
                TResult defaultResult = default
            )
        {
            var result = ReferenceEquals(function, null)
                ? defaultResult
                : function(argument1, argument2, argument3);

            return result;
        }

        #endregion
    }
}
