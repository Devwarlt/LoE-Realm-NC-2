using CA.Threading.Tasks.Procedures;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CA.Threading.Tasks
{
    /// <summary>
    /// Used for synchronous or asynchronous routine.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public sealed class InternalRoutine : IAttachedTask
    {
        private readonly ManualResetEvent resetEvent;
        private readonly Action<int, bool> routine;
        private readonly Stopwatch stopwatch;
        private readonly int ticksPerSecond;
        private readonly int timeout;
        private int delta = 0;
        private bool isCanceled = false;
        private bool started = false;

#pragma warning disable
        private CancellationToken token = default(CancellationToken);

        public InternalRoutine(
            int timeout,
            Action routine
            ) : this(timeout, (delta) => routine(), null) { }

        public InternalRoutine(
            int timeout,
            Action<int> routine,
            Action<string> errorLogger = null
            )

#pragma warning restore

        {
            if (timeout <= 0)
                throw new ArgumentOutOfRangeException("timeout", "Only non-zero and non-negative values are permitted.");

            if (routine == null)
                throw new ArgumentNullException("routine");

            this.timeout = timeout;
            this.routine = (delta, cancel) => { if (!cancel) routine.Invoke(delta); };

            stopwatch = Stopwatch.StartNew();
            ticksPerSecond = 1000 / timeout;
            resetEvent = new ManualResetEvent(false);
            onError += (s, e) =>
            {
                errorLogger?.Invoke(e.ToString());
                Finish();
            };
        }

        /// <summary>
        /// When routine <see cref="timeout"/> takes more time than usual to execute.
        /// </summary>
        public event EventHandler<InternalRoutineEventArgs> OnDeltaVariation;

        /// <summary>
        /// When routine finished its task.
        /// </summary>
        public event EventHandler OnFinished;

        /// <summary>
        /// When routine stars its task.
        /// </summary>
        public event EventHandler OnStarted;

        private event EventHandler<Exception> onError;

        /// <summary>
        /// Get the <see cref="CancellationToken"/> of attached task.
        /// </summary>
        public CancellationToken GetToken => token;

        /// <summary>
        /// Attach a process to parent in case of external task cancellation request.
        /// </summary>
        /// <param name="token"></param>
        public void AttachToParent(CancellationToken token) => this.token = token;

        /// <summary>
        /// Initialize and starts the core routine, to stop it must use <see cref="CancellationTokenSource.Cancel(bool)"/>.
        /// </summary>
        public void Start() => Execute(Loop);

        private Task<long> Execute(Action method)
        {
            Task<long> task = null;

            if (token != default(CancellationToken))
                try
                {
                    isCanceled = token.IsCancellationRequested;

                    token.ThrowIfCancellationRequested();

                    task = Task.Run(() =>
                    {
                        var elapsedMs = stopwatch.ElapsedMilliseconds;

                        method.Invoke();

                        var elapsedMsDelta = stopwatch.ElapsedMilliseconds - elapsedMs;
                        return timeout - elapsedMsDelta;
                    }, token);
                }
                catch (OperationCanceledException) { Finish(); }
            else
                task = Task.Run(() =>
                {
                    var elapsedMs = stopwatch.ElapsedMilliseconds;

                    method.Invoke();

                    var elapsedMsDelta = stopwatch.ElapsedMilliseconds - elapsedMs;
                    return timeout - elapsedMsDelta;
                });

            task?.ContinueWith(t => onError.Invoke(null, t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }

        private void Finish()
        {
            isCanceled = true;
            OnFinished?.Invoke(this, null);
        }

        private void Loop()
        {
            var task = Execute(() => routine.Invoke(delta, isCanceled));

            if (isCanceled || task == null)
                return;

            var result = task.Result < 0 ? 0 : task.Result;

            delta = (int)Math.Max(0, result);

            if (delta > timeout)
                OnDeltaVariation?.Invoke(this, new InternalRoutineEventArgs(delta, ticksPerSecond, timeout));

            resetEvent.WaitOne(delta);

            if (!started)
            {
                started = true;
                OnStarted?.Invoke(this, null);
            }

            Loop();
        }
    }
}
