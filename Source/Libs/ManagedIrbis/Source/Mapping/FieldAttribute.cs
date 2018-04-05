// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FieldAttribute.cs -- устанавливает отображение поля записи на свойство
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
    /// Устанавливает отображение поля записи на свойство объекта.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FieldAttribute
        : Attribute
    {
        #region Properties

        /// <summary>
        /// Метка поля.
        /// </summary>
        public int Tag { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор.
        /// </summary>
        public FieldAttribute
            (
                int tag
            )
        {
            Tag = tag;
        }

        #endregion
    }
}
