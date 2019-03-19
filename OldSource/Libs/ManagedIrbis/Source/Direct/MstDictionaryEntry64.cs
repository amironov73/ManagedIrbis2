// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* MstDictionaryEntry64.cs -- MST file dictionary entry
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Direct
{
    /// <summary>
    /// Элемент справочника MST-файла,
    /// описывающий поле переменной длины.
    /// </summary>
    [PublicAPI]
    public struct MstDictionaryEntry64
    {
        #region Constants

        /// <summary>
        /// Длина элемента справочника MST-файла.
        /// </summary>
        public const int EntrySize = 12;

        #endregion

        #region Properties

        /// <summary>
        /// Field tag.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// Data offset.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Data length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Raw data.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Decoded data.
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString ()
        {
            return $"Tag: {Tag}, Position: {Position}, Length: {Length}, Text: {Text}";
        }

        #endregion
    }
}
