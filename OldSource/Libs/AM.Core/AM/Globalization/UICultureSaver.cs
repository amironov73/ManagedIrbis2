// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* UICultureSaver.cs -- saves and restores current thread UI culture
 * Ars Magna project, http://arsmagna.ru
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
    /// Saves and restores current thread UI culture.
    /// </summary>
    /// <example>
    /// <para>This example changes current thread UI culture to
    /// for a while.
    /// </para>
    /// <code>
    /// using System.Globalization;
    /// using AM.Globalization;
    ///
    /// using ( new UICultureSaver ( "ru-RU" ) )
    /// {
    ///		// do something
    /// }
    /// // here old culture is restored.
    /// </code>
    /// </example>
    [PublicAPI]
    [DebuggerDisplay("{" + nameof(PreviousCulture) + "}")]
    // ReSharper disable once InconsistentNaming
    public sealed class UICultureSaver
        : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the previous UI culture.
        /// </summary>
        [NotNull]
        public CultureInfo PreviousCulture { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Saves current thread UI culture for a while.
        /// </summary>
        public UICultureSaver()
        {
            PreviousCulture = Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Sets new current thread UI culture to the given
        /// <see cref="T:System.Globalization.CultureInfo"/>.
        /// </summary>
        public UICultureSaver
            (
                [NotNull] CultureInfo newCulture
            )
            : this()
        {
            Sure.NotNull(newCulture, nameof(newCulture));

            Thread.CurrentThread.CurrentUICulture = newCulture;
        }

        /// <summary>
        /// Sets current thread UI culture to based on the given name.
        /// </summary>
        public UICultureSaver
            (
                [NotNull] string cultureName
            )
            : this(new CultureInfo(cultureName))
        {
        }

        /// <summary>
        /// Sets current thread UI culture to based on the given identifier.
        /// </summary>
        /// <param name="cultureIdentifier">The culture identifier.</param>
        public UICultureSaver
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
        [NotNull]
        public static UICultureSaver ForTesting()
        {
            return new UICultureSaver(BuiltinCultures.AmericanEnglish);
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Restores old current thread culture.
        /// </summary>
        public void Dispose()
        {
            Thread.CurrentThread.CurrentUICulture = PreviousCulture;
        }

        #endregion
    }
}
