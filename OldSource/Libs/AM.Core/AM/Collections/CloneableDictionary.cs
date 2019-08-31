// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CloneableDictionary.cs -- dictionary of cloneable elements
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Dictionary of cloneable elements.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Count={" + nameof(Count) + "}")]
    public class CloneableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>,
        ICloneable
        where TKey: notnull
    {
        #region ICloneable members

        /// <inheritdoc cref="ICloneable.Clone" />
        public object Clone()
        {
            var result = new CloneableDictionary<TKey, TValue>();

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);
            bool cloneKeys = false;
            bool cloneValues = false;

            if (!keyType.IsValueType)
            {
                if (keyType.IsAssignableFrom(typeof(ICloneable)))
                {
                    Log.Error
                        (
                            "CloneableDictionary::Clone: "
                            + "type "
                            + keyType.FullName
                            + " is not cloneable"
                        );

                    throw new ArgumentException(keyType.Name);
                }
                cloneKeys = true;
            }
            if (!valueType.IsValueType)
            {
                if (valueType.IsAssignableFrom(typeof(ICloneable)))
                {
                    Log.Error
                        (
                            "CloneableDictionary::Clone: "
                            + "type "
                            + valueType.FullName
                            + " is not cloneable"
                        );

                    throw new ArgumentException(valueType.Name);
                }
                cloneValues = true;
            }

            foreach (var pair in this)
            {
                TKey keyCopy = pair.Key;
                TValue valueCopy = pair.Value;
                if (cloneKeys)
                {
                    keyCopy = (TKey)((ICloneable)keyCopy).Clone();
                }
                if (cloneValues
                    && !ReferenceEquals(valueCopy, null))
                {
                    valueCopy = (TValue)((ICloneable)valueCopy).Clone();
                }
                result.Add(keyCopy, valueCopy);
            }

            return result;
        }

        #endregion
    }
}

