// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubFieldMapping.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Reflection;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class SubFieldMapping
    {
        #region Properties

        /// <summary>
        /// Subfield code.
        /// </summary>
        public char Code { get; set; }

        /// <summary>
        /// Property.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Getter function.
        /// </summary>
        public Func<SubField, object> Getter { get; set; }

        ///// <summary>
        ///// Setter function.
        ///// </summary>
        //public Action<object> Backward { get; set; }

        #endregion
    }
}
