// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TextUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using JetBrains.Annotations;

#endregion

namespace AM.Text
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class TextUtility
    {
        #region Public methods

        /// <summary>
        /// Determine kind of the text.
        /// </summary>
        public static TextKind DetermineTextKind
            (
                string? text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return TextKind.PlainText;
            }

            if (text.StartsWith("{") || text.EndsWith("}"))
            {
                return TextKind.RichText;
            }

            if (text.StartsWith("<") || text.EndsWith(">"))
            {
                return TextKind.Html;
            }

            var curly = text.Contains("{") && text.Contains("}");
            var angle = text.Contains("<") && text.Contains(">");

            if (curly && !angle)
            {
                return TextKind.RichText;
            }

            if (angle && !curly)
            {
                return TextKind.Html;
            }

            return TextKind.PlainText;
        }

        #endregion
    }
}
