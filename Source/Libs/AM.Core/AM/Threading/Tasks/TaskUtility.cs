// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* TaskUtility.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

#endregion

namespace AM.Threading.Tasks
{
    /// <summary>
    /// Extensions for <see cref="Task"/> class.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static class TaskUtility
    {
        #region Public methods

        /// <summary>
        /// Сбегание в ThreadPool.
        /// </summary>
        public static ThreadPoolRedirector RedirectToThreadPool() =>
            new ThreadPoolRedirector();

        /// <summary>
        /// Waits for the task to complete, unwrapping any exceptions.
        /// </summary>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        public static void WaitAndUnwrapException
            (
                this Task task
            )
        {
            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Waits for the task to complete, unwrapping any exceptions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of the task.</typeparam>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        /// <returns>The result of the task.</returns>
        public static TResult WaitAndUnwrapException<TResult>
            (
                this Task<TResult> task
            )
        {
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Waits for the task to complete, but does not raise task exceptions. The task exception (if any) is unobserved.
        /// </summary>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        public static void WaitWithoutException
            (
                this Task task
            )
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
            }
        }

        /// <summary>
        /// Waits for the task to complete, but does not raise task exceptions. The task exception (if any) is unobserved.
        /// </summary>
        /// <param name="task">The task. May not be <c>null</c>.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled before the <paramref name="task"/> completed.</exception>
        public static void WaitWithoutException
            (
                this Task task,
                CancellationToken cancellationToken
            )
        {
            try
            {
                task.Wait(cancellationToken);
            }
            catch (AggregateException)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        #endregion
    }
}
