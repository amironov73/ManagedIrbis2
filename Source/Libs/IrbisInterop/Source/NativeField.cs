// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* NativeField.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using AM;

using JetBrains.Annotations;

using ManagedIrbis;

#endregion

namespace IrbisInterop
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class NativeField
    {
        #region Properties

        /// <summary>
        /// Empty array.
        /// </summary>
        [NotNull]
        public static readonly NativeField[] EmptyArray
            = new NativeField[0];

        /// <summary>
        /// Tag.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// Value (with subfields).
        /// </summary>
        public string Value { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Convert <see cref="RecordField"/>
        /// to <see cref="NativeField"/>
        /// </summary>
        [NotNull]
        public static NativeField FromRecordField
            (
                [NotNull] RecordField field
            )
        {
            Sure.NotNull(field, nameof(field));

            NativeField result = new NativeField
            {
                Tag = field.Tag,
                Value = field.ToText()
            };

            return result;
        }

        /// <summary>
        /// Convert <see cref="NativeField"/>
        /// to <see cref="RecordField"/>.
        /// </summary>
        [NotNull]
        public RecordField ToRecordField()
        {
            RecordField result = new RecordField
                (
                    Tag.ToInvariantString(),
                    Value
                );

            return result;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return string.Format
                (
                    "{0}: {1}",
                    Tag.ToInvariantString(),
                    Value.ToVisibleString()
                );
        }

        #endregion
    }
}
