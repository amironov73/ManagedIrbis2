// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextPosition.cs -- cursor position in the TextNavigator
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System.Diagnostics;

using JetBrains.Annotations;

#endregion

namespace AM.Text
{
    /// <summary>
    /// Cursor position in <see cref="TextNavigator"/>.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Column={Column} Line={Line} Position={Position}")]
    public struct TextPosition
    {
        #region Properties

        /// <summary>
        /// Column number.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Absolute position.
        /// </summary>
        public int Position { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextPosition
            (
                [NotNull] TextNavigator navigator
            )
        {
            Sure.NotNull(navigator, nameof(navigator));

            Column = navigator.Column;
            Line = navigator.Line;
            Position = navigator.Position;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextPosition
            (
                [NotNull] TextNavigator2 navigator
            )
        {
            Sure.NotNull(navigator, nameof(navigator));

            Column = navigator.Column;
            Line = navigator.Line;
            Position = navigator.Position;
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        [Pure]
        public override string ToString()
        {
            return $"Line={Line}, Column={Column}";
        }

        #endregion
    }
}
