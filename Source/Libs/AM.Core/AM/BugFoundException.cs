// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BugFoundException.cs -- thrown when something went wrong
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Thrown when something went wrong.
    /// </summary>
    [PublicAPI]
    public class BugFoundException
        : ArsMagnaException
    {
        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BugFoundException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        public BugFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public BugFoundException
            (
                string message,
                Exception innerException
            )
            : base(message, innerException)
        {
        }

        #endregion
    }
}
