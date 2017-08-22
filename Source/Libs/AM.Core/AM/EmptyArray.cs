// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* EmptyArray.cs -- empty array shared instance to reduce memory traffic
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Empty array shared instance to reduce memory traffic.
    /// </summary>
    [PublicAPI]
    public static class EmptyArray<T>
    {
        #region Fields

        /// <summary>
        /// Shared instance.
        /// </summary>
        [NotNull]
        public static readonly T[] Value = new T[0];

        #endregion
    }
}
