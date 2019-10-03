// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RuntimeUtility.cs -- some useful methods for runtime
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// Some useful methods for runtime.
    /// </summary>
    [PublicAPI]
    public static class RuntimeUtility
    {
        #region Properties

        /// <summary>
        /// Путь к файлам текущей версии Net Framework.
        /// </summary>
        /// <remarks>
        /// Типичная выдача:
        /// C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0
        /// </remarks>
        public static string FrameworkLocation
        {
            get
            {
                var result = Path.GetDirectoryName(typeof(int).Assembly.Location);
                if (string.IsNullOrEmpty(result))
                {
                    throw new ArsMagnaException("Can't determine framework location");
                }

                return result;
            }
        }

        /// <summary>
        /// Имя исполняемого процесса.
        /// </summary>
        /// <remarks>
        /// В .NET Core типичная выдача:
        /// C:\Program Files\dotnet\dotnet.exe
        /// </remarks>
        public static string? ExecutableFileName
        {
            get
            {
                var process = Process.GetCurrentProcess();
                var module = process.MainModule;

                return module?.FileName;
            }
        }

        /// <summary>
        /// Приложение запущено на Windows?
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static bool IsWindows
        {
            [DebuggerStepThrough]
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Pre-JIT types of the <paramref name="assembly"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static void PrepareAssembly
            (
                Assembly assembly
            )
        {
            Sure.NotNull(assembly, nameof(assembly));

            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    PrepareType(type);
                }
            }
            catch (Exception exception)
            {
                Log.TraceException
                    (
                        nameof(RuntimeUtility) + "::" + nameof(PrepareAssembly),
                        exception
                    );
            }
        }

        /// <summary>
        /// Pre-JIT methods of the <paramref name="type"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static void PrepareType
            (
                Type type
            )
        {
            Sure.NotNull(type, nameof(type));

            try
            {
                foreach (var method in type.GetMethods())
                {
                    RuntimeHelpers.PrepareMethod(method.MethodHandle);
                }
            }
            catch (Exception exception)
            {
                Log.TraceException
                    (
                        nameof(RuntimeUtility) + "::" + nameof(PrepareType),
                        exception
                    );
            }
        }

        #endregion
    }
}

