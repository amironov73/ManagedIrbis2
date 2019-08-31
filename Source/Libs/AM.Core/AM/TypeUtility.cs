// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TypeUtility.cs -- type-info related useful routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//using AM.Reflection;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Collection of useful type-info related routines.
    /// </summary>
    [PublicAPI]
    public static class TypeUtility
    {
        #region Public methods

        /// <summary>
        /// Gets type of the argument.
        /// </summary>
        public static Type GetType<T>
            (
                T arg
            )
            where T : class
        {
            return ReferenceEquals(arg, null)
                ? typeof(T)
                : arg.GetType();
        }

        #endregion
    }
}
