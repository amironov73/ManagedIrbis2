// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CloneableCollection.cs -- collection of cloneable elements
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: moderate
 */

#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using JetBrains.Annotations;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Collection of cloneable elements.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class CloneableCollection<T>
        : Collection<T>,
        ICloneable
    {
        #region ICloneable members

        /// <inheritdoc cref="ICloneable.Clone" />
        public object Clone()
        {
            var result = new CloneableCollection<T>();

            foreach (var item in this)
            {
                result.Add(item);
            }

            return result;
        }

        #endregion
    }
}
