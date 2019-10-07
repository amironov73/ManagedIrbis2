// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* CultureSaver.cs -- saves and restores current thread culture
 * Ars Magna project, https://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

using JetBrains.Annotations;

#endregion

namespace AM.Globalization
{
    /// <summary>
    /// Saves and restores current thread culture.
    /// </summary>
    /// <example>
    /// <para>This example changes current thread culture to
    /// for a while.
    /// </para>
    /// <code>
    /// using System.Globalization;
    /// using AM.Globalization;
    ///
    /// using ( new CultureSaver ( "ru-RU" ) )
    /// {
    ///     // do something
    /// }
    /// // here old culture is restored.
    /// </code>
    /// </example>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(PreviousCulture) + "}")]
    public sealed class CultureSaver
        : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the previous culture.
        /// </summary>
        public CultureInfo PreviousCulture { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Saves current thread culture for a while.
        /// </summary>
        public CultureSaver()
        {
            PreviousCulture = Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Sets new current thread culture to the given
        /// <see cref="T:System.Globalization.CultureInfo"/>.
        /// </summary>
        public CultureSaver
            (
                CultureInfo newCulture
            )
            : this()
        {
            Thread.CurrentThread.CurrentCulture = newCulture;
        }

        /// <summary>
        /// Sets current thread culture to based on the given name.
        /// </summary>
        /// <param name="cultureName">Name of the culture.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="cultureName"/> is <c>null</c>.
        /// </exception>
        public CultureSaver
            (
                string cultureName
            )
            : this(new CultureInfo(cultureName))
        {
        }

        /// <summary>
        /// Sets current thread culture to based on the given identifier.
        /// </summary>
        /// <param name="cultureIdentifier">The culture identifier.</param>
        public CultureSaver
            (
                int cultureIdentifier
            )
            : this(new CultureInfo(cultureIdentifier))
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Temporary switch current thread culture for testing purposes.
        /// </summary>
        public static CultureSaver ForTesting()
        {
            return new CultureSaver(BuiltinCultures.AmericanEnglish);
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Restores old current thread UI culture.
        /// </summary>
        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = PreviousCulture;
        }

        #endregion
    }
}
