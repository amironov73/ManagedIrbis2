// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* LoggingClientSocket.cs -- logging socket for debug
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AM;
using AM.IO;
using AM.Logging;

using JetBrains.Annotations;

using ManagedIrbis.Properties;

#endregion

namespace ManagedIrbis.Infrastructure.Sockets
{
    /// <summary>
    /// Logging socket for debug purposes.
    /// </summary>
    [PublicAPI]
    public sealed class LoggingClientSocket
        : ClientSocket
    {
        #region Properties

        /// <summary>
        /// Path to store debug data.
        /// </summary>
        [NotNull]
        public string DebugPath { get; }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoggingClientSocket
            (
                [NotNull] ClientSocket innerSocket,
                [NotNull] string debugPath
            )
        {
            Sure.NotNull(innerSocket, nameof(innerSocket));
            Sure.NotNullNorEmpty(debugPath, nameof(debugPath));

            if (!Directory.Exists(debugPath))
            {
                throw new IrbisNetworkException(Resources.DirectoryNotExist + debugPath);
            }

            DebugPath = debugPath;
            InnerSocket = innerSocket;
        }

        #endregion

        #region Private members

        private static int _counter;

        private void _DumpGeneralInfo
            (
                string suffix,
                string text
            )
        {
            int counter = Interlocked.Increment(ref _counter);

            string path = Path.Combine
                (
                    DebugPath,
                    $"{counter:00000000}{suffix}.packet"
                );
            File.WriteAllText(path, text);
        }

        private void _DumpException
            (
                [NotNull] Exception exception
            )
        {
            _DumpGeneralInfo
                (
                    "ex",
                    exception.ToString()
                );
        }

        private void _DumpPackets
            (
                [NotNull] byte[][] request,
                [NotNull] byte[] answer
            )
        {
            int counter = Interlocked.Increment(ref _counter);

            string upPath = Path.Combine
                (
                    DebugPath,
                    $"{counter:00000000}up.packet"
                );
            using (FileStream stream = new FileStream(upPath, FileMode.CreateNew))
            {
                foreach (byte[] bytes in request)
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            string downPath = Path.Combine
                (
                    DebugPath,
                    $"{counter:00000000}dn.packet"
                );
            File.WriteAllBytes(downPath, answer);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Set start value for packet counter.
        /// </summary>
        public static void SetCounter
            (
                int startValue
            )
        {
            Sure.NonNegative(startValue, nameof(startValue));

            _counter = startValue;
        }

        /// <summary>
        /// Do some setup before using.
        /// </summary>
        public static void Setup
            (
                [NotNull] string debugPath,
                bool clearDirectory
            )
        {
            Sure.NotNullNorEmpty(debugPath, nameof(debugPath));

            if (!Directory.Exists(debugPath))
            {
                Directory.CreateDirectory(debugPath);
            }

            if (clearDirectory)
            {
                DirectoryUtility.ClearDirectory(debugPath);
            }
            else
            {
                _counter = Directory.GetFiles(debugPath).Length;
            }
        }

        #endregion

        #region ClientSocket members

        /// <inheritdoc cref="ClientSocket.AbortRequest" />
        public override void AbortRequest()
        {
            InnerSocket
                .ThrowIfNull(nameof(InnerSocket))
                .AbortRequest();
        }

        /// <inheritdoc cref="ClientSocket.ExecuteRequest" />
        public override void ExecuteRequest
            (
                ClientContext context
            )
        {
            Sure.NotNull(context, nameof(context));

            ClientSocket innerSocket = InnerSocket
                .ThrowIfNull(nameof(InnerSocket));

            try
            {
                innerSocket.ExecuteRequest(context);
            }
            catch (Exception exception)
            {
                Log.TraceException
                    (
                        nameof(LoggingClientSocket) + "::" + nameof(ExecuteRequest),
                        exception
                    );

                Task.Factory.StartNew(() => _DumpException(exception));
                throw;
            }

            Task.Factory.StartNew
                (
                    () => _DumpPackets(context.RawQuery, context.RawResponse)
                );
        }

        #endregion
    }
}

