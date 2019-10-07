// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* PlatformAbstractionLayer.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;

using JetBrains.Annotations;

#endregion

namespace AM.PlatformAbstraction
{
    /// <summary>
    /// Platform abstraction level.
    /// </summary>
    [PublicAPI]
    public class PlatformAbstractionLayer
        : IDisposable
    {
        #region Public methods

        /// <summary>
        /// Exit.
        /// </summary>
        public virtual void Exit
            (
                int exitCode
            )
        {
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Fail fast.
        /// </summary>
        public virtual void FailFast
            (
                string message
            )
        {
            Environment.FailFast(message);
        }

        /// <summary>
        /// Get environment variable.
        /// </summary>
        public virtual string? GetEnvironmentVariable
            (
                string variableName
            )
        {
            return Environment.GetEnvironmentVariable(variableName);
        }

        /// <summary>
        /// Get the machine name.
        /// </summary>
        public virtual string GetMachineName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// Get random number generator.
        /// </summary>
        public virtual Random GetRandomGenerator()
        {
            return new Random();
        }

        /// <summary>
        /// Get current date and time.
        /// </summary>
        public virtual DateTime Now()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Get the operating system version.
        /// </summary>
        public virtual OperatingSystem OsVersion()
        {
            return Environment.OSVersion;
        }

        /// <summary>
        /// Get today date.
        /// </summary>
        public virtual DateTime Today()
        {
            return DateTime.Today;
        }

        #endregion

        #region IDisposable members

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            // Nothing to do here
        }

        #endregion
    }
}
