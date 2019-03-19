﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* Operator.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Linq.Expressions;

using JetBrains.Annotations;

#endregion

// ReSharper disable once InconsistentNaming

namespace AM
{
    /// <summary>
    /// Experimental operator helpers.
    /// </summary>
    [PublicAPI]
    public class Operator<T>
    {
        #region Public methods

        /// <summary>
        /// Operator "new".
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public static Func<T> New { get { return _new; } }

        #endregion

        #region Private members

        private static readonly Func<T> _new = Expression.Lambda<Func<T>>
            (
                Expression.New(typeof(T))
            )
            .Compile();

        #endregion
    }
}
