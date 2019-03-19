﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CloneableDictionary.cs -- cloneable dictionary
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Cloneable dictionary.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Count={Count}")]
    public class CloneableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>,
        ICloneable
    {
        #region ICloneable members

        /// <inheritdoc cref="ICloneable.Clone" />
        public object Clone()
        {
            CloneableDictionary<TKey, TValue> result
                = new CloneableDictionary<TKey, TValue>();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                result.Add(pair.Key, pair.Value);
            }

            //Type keyType = typeof(TKey);
            //Type valueType = typeof(TValue);
            //bool cloneKeys = false;
            //bool cloneValues = false;

            //if (!TypeUtility.IsValueType(keyType))
            //{
            //    if (keyType.Bridge().IsAssignableFrom(typeof(ICloneable)))
            //    {
            //        Log.Error
            //            (
            //                "CloneableDictionary::Clone: "
            //                + "type "
            //                + keyType.FullName
            //                + " is not cloneable"
            //            );

            //        throw new ArgumentException(keyType.Name);
            //    }
            //    cloneKeys = true;
            //}
            //if (!TypeUtility.IsValueType(valueType))
            //{
            //    if (valueType.Bridge().IsAssignableFrom(typeof(ICloneable)))
            //    {
            //        Log.Error
            //        (
            //            "CloneableDictionary::Clone: "
            //            + "type "
            //            + valueType.FullName
            //            + " is not cloneable"
            //        );

            //        throw new ArgumentException(valueType.Name);
            //    }
            //    cloneValues = true;
            //}

            //foreach (KeyValuePair<TKey, TValue> pair in this)
            //{
            //    TKey keyCopy = pair.Key;
            //    TValue valueCopy = pair.Value;
            //    if (cloneKeys)
            //    {
            //        keyCopy = (TKey)((ICloneable)pair.Key).Clone();
            //    }
            //    if (cloneValues
            //        && !ReferenceEquals(valueCopy, null))
            //    {
            //        valueCopy = (TValue)((ICloneable)pair.Value).Clone();
            //    }
            //    result.Add(keyCopy, valueCopy);
            //}

            return result;
        }

        #endregion
    }
}

