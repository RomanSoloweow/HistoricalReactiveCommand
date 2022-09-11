using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;


namespace HistoricalReactiveCommand
{
    public static partial class ReactiveCommandWithHistoryEx
    {
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, CancellationToken, Task<TResult>> execute,
            Func<TParam, TResult, CancellationToken, Task<TResult>> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }


            return CreateWithHistoryFromObservable<TParam, TResult>(commandKey,
                (param,result) => Observable.StartAsync(ct => execute(param, result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, result, ct)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, Task<TResult>> execute,
            Func<TParam, TResult, Task<TResult>> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<TParam, TResult>(commandKey,
                    (param, result) => Observable.StartAsync(ct => execute(param, result)),
                    (param, result) => Observable.StartAsync(ct => discard(param, result)),
                    historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
            Func<TParam, Task> execute,
            Func<TParam, Task> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<TParam, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(param)),
                (param, result) => Observable.StartAsync(ct => discard(param)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, CancellationToken, Task> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<TParam, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(param, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, ct)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            string commandKey,
            Func<Task> execute,
            Func<Task> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<Unit, Unit>(commandKey,
                    (param, result) => Observable.StartAsync(ct => execute()),
                    (param, result) => Observable.StartAsync(ct => discard()),
                    historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            string commandKey,
            Func<CancellationToken, Task> execute,
            Func<CancellationToken, Task> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<Unit, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(ct)),
                (param, result) => Observable.StartAsync(ct => discard(ct)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
            Func<TResult, Task<TResult>> execute,
            Func<TResult, Task<TResult>> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<Unit, TResult>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(result)),
                (param, result) => Observable.StartAsync(ct => discard(result)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
            Func<TResult, CancellationToken, Task<TResult>> execute,
            Func<TResult, CancellationToken, Task<TResult>> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable<Unit, TResult>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(result, ct)),
                historyContext, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistory<TParam>(
            string commandKey,
            Action<TParam> execute,
            Action<TParam> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistory<TParam, Unit>(commandKey,
                (param, result) => { execute(param); return Unit.Default; },
                (param, result) => { discard(param); return Unit.Default; },
                historyContext,canExecute, outputScheduler);

        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory(
            string commandKey,
            Action execute,
            Action discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistory<Unit, Unit>(commandKey,
                    (param, result) => { execute(); return Unit.Default;},
                    (param, result) => { discard(); return Unit.Default; },
                    historyContext, canExecute, outputScheduler);

        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistory<TResult>(
            string commandKey,
            Func<TResult, TResult> execute,
            Func<TResult, TResult> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistory<Unit, TResult>(commandKey,
                (param, result)=> execute(result),
                (param, result) => discard(result),
                historyContext, canExecute, outputScheduler);
        }
    }
}