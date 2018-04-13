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
    public sealed class SubFieldMapping<T>
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
        /// Forward converter.
        /// </summary>
        public Action<RecordField, T> ForwardConverter { get; set; }

        /// <summary>
        /// Backward converter.
        /// </summary>
        public Action<T, RecordField> BackwardConverter { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Ensure converters was created.
        /// </summary>
        [NotNull]
        public SubFieldMapping<T> EnsureConvertersCreated()
        {
            if (ReferenceEquals(ForwardConverter, null))
            {
                ForwardConverter = MappingUtility.CreateForwardConverter(this);
                BackwardConverter = MappingUtility.CreateBackwardConverter(this);
            }

            return this;
        }

        #endregion
    }
}
