// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ThreadPoolRedirector.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Runtime.CompilerServices;
using System.Threading;

#endregion

namespace AM.Threading.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreadPoolRedirector
        : INotifyCompletion
    {
        #region INotifyCompletion members

        /// <summary>
        /// awaiter и awaitable в одном флаконе
        /// </summary>
        public ThreadPoolRedirector GetAwaiter() => this;

        /// <summary>
        /// true означает выполнять продолжение немедленно
        /// </summary>
        public bool IsCompleted => Thread.CurrentThread.IsThreadPoolThread;

        /// <inheritdoc cref="INotifyCompletion.OnCompleted" />
        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(o => continuation());
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetResult() { }

        #endregion
    }
}
