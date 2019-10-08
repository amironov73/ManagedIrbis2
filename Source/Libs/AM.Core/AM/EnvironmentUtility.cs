// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* EnvironmentUtility.cs -- program environment study routines
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using AM.Text;

//using AM.Reflection;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Program environment study routines.
    /// </summary>
    [PublicAPI]
    public static class EnvironmentUtility
    {
        /// <summary>
        /// Gets a value indicating whether system is 32-bit.
        /// </summary>
        /// <value><c>true</c> if system is 32-bit; otherwise,
        /// <c>false</c>.</value>
        [ExcludeFromCodeCoverage]
        public static bool Is32Bit
        {
            [DebuggerStepThrough]
            get => IntPtr.Size == 4;
        }

        /// <summary>
        /// Gets a value indicating whether system is 64-bit.
        /// </summary>
        /// <value><c>true</c> if system is 64-bit; otherwise,
        /// <c>false</c>.</value>
        [ExcludeFromCodeCoverage]
        public static bool Is64Bit
        {
            [DebuggerStepThrough]
            get => IntPtr.Size == 8;
        }

        /// <summary>
        /// Running on Mono?
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static bool IsMono ()
        {
            return !ReferenceEquals(Type.GetType ("Mono.Runtime"), null);
        }

        /// <summary>
        /// Get .NET Core version.
        /// </summary>
        public static Version NetCoreVersion()
        {
            var description = RuntimeInformation.FrameworkDescription;
            var parts = description.Split(CommonSeparators.Space);
            return Version.Parse(parts[2]);
        }

        /// <summary>
        /// Optimal degree of parallelism.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static int OptimalParallelism
        {
            get
            {
                int result = Math.Min
                    (
                        Math.Max
                            (
                                Environment.ProcessorCount - 1,
                                1
                            ),
                        8 // TODO choose good number
                    );

                return result;
            }
        }

        /// <summary>
        /// System uptime.
        /// </summary>
        public static TimeSpan Uptime
        {
            [DebuggerStepThrough]
            get => new TimeSpan(Environment.TickCount);
        }
    }
}
