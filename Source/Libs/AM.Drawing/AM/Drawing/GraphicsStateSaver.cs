// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* GraphicsStateSaver.cs -- simple holder of graphics context state.
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using JetBrains.Annotations;

#endregion

namespace AM.Drawing
{
    /// <summary>
    /// Holds state of <see cref="T:System.Drawing.Graphics"/>
    /// class.
    /// </summary>
    [PublicAPI]
    public sealed class GraphicsStateSaver
        : IDisposable
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:GraphicsStateSaver"/> class.
        /// </summary>
        public GraphicsStateSaver
            (
                [NotNull] Graphics graphics
            )
        {
            Sure.NotNull(graphics, nameof(graphics));

            _graphics = graphics;
            _state = _graphics.Save();
        }

        #endregion

        #region Private members

        /// <summary>
        /// Object of <see cref="T:System.Drawing.Graphics"/> type
        /// which state have been saved.
        /// </summary>
        private readonly Graphics _graphics;

        /// <summary>
        /// Saved state itself.
        /// </summary>
        private GraphicsState _state;

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c>
        /// [disposing].</param>
        private void Dispose
            (
                bool disposing
            )
        {
            if (_state != null)
            {
                _graphics.Restore(_state);
                _state = null;
            }
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
