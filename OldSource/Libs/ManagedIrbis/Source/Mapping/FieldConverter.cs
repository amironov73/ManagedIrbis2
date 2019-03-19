// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* FieldAttribute.cs -- устанавливает отображение поля записи на свойство
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AM;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    /// Converts <see cref="RecordField"/> to the object of given type.
    /// </summary>
    /// <typeparam name="T">Type of the object.</typeparam>
    [PublicAPI]
    public class FieldConverter<T>
        where T: class
    {
        #region Properties

        /// <summary>
        /// Mappings.
        /// </summary>
        [NotNull]
        public List<SubFieldMapping<T>> Mappings { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public FieldConverter()
        {
            Mappings = new List<SubFieldMapping<T>>();
        }

        #endregion

        #region Private members

        [NotNull]
        private SubFieldMapping<T> _BuildMapping
            (
                [NotNull] PropertyInfo property
            )
        {
            SubFieldMapping<T> result = new SubFieldMapping<T>
            {
                Property = property
            };

            SubFieldAttribute attribute
                = property.GetCustomAttribute<SubFieldAttribute>()
                .ThrowIfNull(nameof(attribute));
            result.Code = attribute.Code;

            return result;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Build the <see cref="Mappings"/>.
        /// </summary>
        public void BuildMappings()
        {
            Mappings.Clear();

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties
                (
                    BindingFlags.Instance
                    | BindingFlags.Public
                );
            Mappings.AddRange(properties.Select(_BuildMapping));
        }

        /// <summary>
        /// Do the conversion from the field to the object with properties.
        /// </summary>
        public void ConvertFromField
            (
                [NotNull] RecordField field,
                [NotNull] T output
            )
        {
            Sure.NotNull(field, nameof(field));
            Sure.NotNull(output, nameof(output));

            foreach (SubFieldMapping<T> mapping in Mappings)
            {
                SubField subField = field.GetFirstSubField(mapping.Code);
                if (!ReferenceEquals(subField, null))
                {
                    object value = null;//mapping.Getter(subField);
                    mapping.Property.SetValue(output, value);
                }
            }
        }

        /// <summary>
        /// Do the conversion from the object to the field.
        /// </summary>
        public void ConvertToField
            (
                [NotNull] T input,
                [NotNull] RecordField field
            )
        {
            Sure.NotNull(input, nameof(input));
            Sure.NotNull(field, nameof(field));

            foreach (SubFieldMapping<T> mapping in Mappings)
            {
                object value = mapping.Property.GetValue(field);
                if (ReferenceEquals(value, null))
                {
                    field.RemoveSubField(mapping.Code);
                }
                else
                {
                    field.SetSubField(mapping.Code, value);
                }
            }
        }

        #endregion
    }
}
