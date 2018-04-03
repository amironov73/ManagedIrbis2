// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubFieldAttribute.cs -- устанавливает отображение подполя на свойство объекта
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Устанавливает отображение подполя на свойство объекта.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SubFieldAttribute
        : Attribute
    {
        #region Properties

        /// <summary>
        /// Код подполя.
        /// </summary>
        public char Code { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="code"></param>
        public SubFieldAttribute
            (
                char code
            )
        {
            Code = code;
        }

        #endregion
    }
}
