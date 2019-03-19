// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* BusyState.cs -- busy state indicator
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AM.Logging;
using AM.Runtime;

using JetBrains.Annotations;

#endregion

namespace AM.Threading
{
    /// <summary>
    /// Busy state indicator.
    /// </summary>
    [PublicAPI]
    [DebuggerDisplay("Busy = {" + nameof(Busy) + "}")]
    public sealed class BusyState
        : IHandmadeSerializable
    {
        #region Events

        /// <summary>
        /// Raised when the state has changed.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region Properties

        /// <summary>
        /// The state itself.
        /// </summary>
        public bool Busy => _currentState;

        /// <summary>
        /// Whether to use asynchronous event handler.
        /// </summary>
        public bool UseAsync { get; set; }

        /// <summary>
        /// Хэндл для ожидания.
        /// </summary>
        public WaitHandle WaitHandle => _waitHandle;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public BusyState()
        {
            Log.Trace("BusyState::Constructor");

            _lock = new object();
            _waitHandle = new ManualResetEvent(true);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BusyState
            (
                bool initialState
            )
            : this()
        {
            _currentState = initialState;
        }

        #endregion

        #region Private members

        private readonly object _lock;

        private bool _currentState;

#if !UAP

        private Thread _thread;

#endif

        private ManualResetEvent _waitHandle;

        #endregion

        #region Public methods

        /// <summary>
        /// Run some code.
        /// </summary>
        public void Run
            (
                [NotNull] Action action
            )
        {
            Sure.NotNull(action, nameof(action));

            WaitAndGrab();

            try
            {
                action();
            }
            finally
            {
                SetState(false);
            }
        }

        /// <summary>
        /// Run some code in asychronous manner.
        /// </summary>
        public Task RunAsync
            (
                [NotNull] Action action
            )
        {
            Sure.NotNull(action, nameof(action));

            Task result = Task.Factory.StartNew
                (
                    () => Run(action)
                );

            return result;
        }

        /// <summary>
        /// Change the state.
        /// </summary>
        public void SetState
            (
                bool newState
            )
        {
            Log.Trace
                (
                    nameof(BusyState) + "::" + nameof(SetState)
                    + ": newState="
                    + newState
                );

            lock (_lock)
            {
                if (newState != _currentState)
                {
                    if (newState)
                    {
                        _waitHandle.Reset();
                        _thread = Thread.CurrentThread;
                    }
                    else
                    {
                        _waitHandle.Set();
                    }

                    _currentState = newState;

                    if (UseAsync)
                    {
                        StateChanged.RaiseAsync(this);
                    }
                    else
                    {
                        StateChanged.Raise(this);
                    }
                }
            }
        }

        /// <summary>
        /// Ожидаем, пока не освободится, и захватываем.
        /// </summary>
        public void WaitAndGrab()
        {
            lock (_lock)
            {
                while (true)
                {
                    if (!Busy)
                    {
                        SetState(true);
                        goto DONE;
                    }

                    if (!ReferenceEquals(_thread, Thread.CurrentThread))
                    {
                        WaitHandle.WaitOne();
                    }
                }
            }

        DONE:
            Log.Trace(nameof(BusyState) + "::" + nameof(WaitAndGrab) + ": return");
        }

        /// <summary>
        /// Ожидаем, пока не освободится, затем захватываем.
        /// </summary>
        public bool WaitAndGrab
            (
                TimeSpan timeout
            )
        {
            lock (_lock)
            {
                if (!Busy)
                {
                    SetState(true);
                    return true;
                }

                bool result;

                if (ReferenceEquals(_thread, Thread.CurrentThread))
                {
                    result = true;
                }
                else
                {
                    result = WaitHandle.WaitOne(timeout);
                }

                if (result)
                {
                    SetState(true);
                }

                return result;
            }
        }

        /// <summary>
        /// Ожидаем, пока не освободится.
        /// </summary>
        public void WaitFreeState()
        {
            while (true)
            {
                if (!Busy)
                {
                    goto DONE;
                }

                if (ReferenceEquals(_thread, Thread.CurrentThread))
                {
                    goto DONE;
                }

                WaitHandle.WaitOne();
            }

        DONE:
            Log.Trace(nameof(BusyState) + "::" + nameof(WaitFreeState) + ": return");
        }

        /// <summary>
        /// Ожидаем, пока не освободится.
        /// </summary>
        public bool WaitFreeState
            (
                TimeSpan timeout
            )
        {
            if (!Busy)
            {
                return true;
            }

            if (ReferenceEquals(_thread, Thread.CurrentThread))
            {
                return true;
            }

            return WaitHandle.WaitOne(timeout);
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        public static implicit operator bool
            (
                [NotNull] BusyState state
            )
        {
            return state.Busy;
        }

        /// <summary>
        /// Implicit conversion operator.
        /// </summary>
        public static implicit operator BusyState
            (
                bool value
            )
        {
            return new BusyState(value);
        }

        #endregion

        #region IHandmadeSerializable members

        /// <inheritdoc cref="IHandmadeSerializable.RestoreFromStream" />
        public void RestoreFromStream
            (
                BinaryReader reader
            )
        {
            _currentState = reader.ReadBoolean();
            UseAsync = reader.ReadBoolean();
        }

        /// <inheritdoc cref="IHandmadeSerializable.SaveToStream" />
        public void SaveToStream
            (
                BinaryWriter writer
            )
        {
            writer.Write(_currentState);
            writer.Write(UseAsync);
        }

        #endregion

        #region Object members

        /// <inheritdoc cref="object.ToString" />
        public override string ToString()
        {
            return string.Format("Busy: {0}", Busy);
        }

        #endregion
    }
}
