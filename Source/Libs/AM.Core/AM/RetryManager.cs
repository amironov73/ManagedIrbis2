// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* RetryManager.cs -- retry execution of function
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Threading;
using System.Threading.Tasks;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM
{
    /// <summary>
    /// Retry execution of function for specified number of times.
    /// </summary>
    [PublicAPI]
    public sealed class RetryManager
    {
        #region Events

        /// <summary>
        /// Raised when exception occurs.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionOccurs;

        /// <summary>
        /// Raised when exception is resolved.
        /// </summary>
        public event EventHandler Resolved;

        #endregion

        #region Properties

        /// <summary>
        /// Delay interval, milliseconds.
        /// </summary>
        public int DelayInterval { get; set; }

        /// <summary>
        /// Retry count.
        /// </summary>
        public int RetryLimit { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Construction.
        /// </summary>
        public RetryManager
            (
                int retryLimit
            )
        {
            Sure.Positive(retryLimit, "retryLimit");

            RetryLimit = retryLimit;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RetryManager
            (
                int retryLimit,
                [CanBeNull] Func<Exception, bool> resolver
            )
        {
            Sure.Positive(retryLimit, "retryLimit");

            RetryLimit = retryLimit;
            _resolver = resolver;
        }

        #endregion

        #region Private members

        private readonly Func<Exception, bool> _resolver;

        private void _Delay()
        {
            if (DelayInterval > 0)
            {
                Thread.Sleep(DelayInterval);
            }
        }

        private void _Resolve
            (
                int count,
                Exception ex
            )
        {
            Log.Error
                (
                    "RetryManager::_Resolve: "
                    + "catch exception: "
                    + ex.GetType().Name
                    + ": "
                    + ex.Message
                    + ", count="
                    + count
                );

            if (count >= RetryLimit)
            {
                Log.Error
                    (
                        "RetryManager::_Resolve: "
                        + "count exceeded limit="
                        + RetryLimit
                    );

                throw ex;
            }

            ExceptionEventArgs eventArgs
                = new ExceptionEventArgs (ex);
            ExceptionOccurs.Raise(this, eventArgs);

            if (ReferenceEquals(_resolver, null))
            {
                return;
            }

            bool result = _resolver(ex);
            if (!result)
            {
                Log.Error
                    (
                        "RetryManager::_Resolve: "
                        + "couldn't resolve: "
                        + ex.GetType().Name
                    );

                throw new ArsMagnaException
                    (
                        "RetryManager failed",
                        ex
                    );
            }

            Resolved.Raise(this);

            _Delay();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public void Try
            (
                [NotNull] Action action
            )
        {
            Sure.NotNull(action, "action");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public void Try<T>
            (
                [NotNull] Action<T> action,
                T argument
            )
        {
            Sure.NotNull(action, "action");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    action(argument);
                    return;
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public void Try<T1,T2>
            (
                [NotNull] Action<T1,T2> action,
                T1 argument1,
                T2 argument2
            )
        {
            Sure.NotNull(action, nameof(action));

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    action(argument1, argument2);
                    return;
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public void Try<T1, T2, T3>
            (
                [NotNull] Action<T1, T2, T3> action,
                T1 argument1,
                T2 argument2,
                T3 argument3
            )
        {
            Sure.NotNull(action, "action");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    action(argument1, argument2, argument3);
                    return;
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public T Try<T>
            (
                [NotNull] Func<T> function
            )
        {
            Sure.NotNull(function, "function");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    return function();
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public TResult Try<T1,TResult>
            (
                [NotNull] Func<T1,TResult> function,
                T1 argument
            )
        {
            Sure.NotNull(function, "function");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    return function(argument);
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public TResult Try<T1, T2, TResult>
            (
                [NotNull] Func<T1, T2, TResult> function,
                T1 argument1,
                T2 argument2
            )
        {
            Sure.NotNull(function, "function");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    return function(argument1, argument2);
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        /// <summary>
        /// Try to execute specified function.
        /// </summary>
        public TResult Try<T1, T2, T3, TResult>
            (
                [NotNull] Func<T1, T2, T3, TResult> function,
                T1 argument1,
                T2 argument2,
                T3 argument3
            )
        {
            Sure.NotNull(function, "function");

            for (int i = 0; i <= RetryLimit; i++)
            {
                try
                {
                    return function(argument1, argument2, argument3);
                }
                catch (Exception ex)
                {
                    _Resolve(i, ex);
                }
            }

            Log.Error
                (
                    "RetryManager::Try: "
                    + "giving up"
                );

            throw new ArsMagnaException("RetryManager failed");
        }

        #endregion
    }
}
