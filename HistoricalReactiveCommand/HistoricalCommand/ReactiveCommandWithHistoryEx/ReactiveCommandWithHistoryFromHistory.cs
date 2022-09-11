using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HistoricalReactiveCommand.Imports;
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
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, Task<TResult>> execute,
            Func<TParam, TResult, Task<TResult>> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                     history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
            Func<TParam, Task> execute,
            Func<TParam, Task> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, CancellationToken, Task> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            string commandKey,
            Func<Task> execute,
            Func<Task> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                    history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            string commandKey,
            Func<CancellationToken, Task> execute,
            Func<CancellationToken, Task> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
            Func<TResult, Task<TResult>> execute,
            Func<TResult, Task<TResult>> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
            Func<TResult, CancellationToken, Task<TResult>> execute,
            Func<TResult, CancellationToken, Task<TResult>> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistory<TParam>(
            string commandKey,
            Action<TParam> execute,
            Action<TParam> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history,canExecute, outputScheduler);

        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory(
            string commandKey,
            Action execute,
            Action discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                    history, canExecute, outputScheduler);

        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistory<TResult>(
            string commandKey,
            Func<TResult, TResult> execute,
            Func<TResult, TResult> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
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
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistory(
                commandKey, execute, discard,
                HistoryContextEx.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistory history,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            return CreateWithHistoryFromObservable(
                commandKey, execute, discard, 
                HistoryContextEx.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>
        (
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            HistoryContext context,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null
        )
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return new ReactiveCommandWithHistory<TParam, TResult>(commandKey,
                  (param, result) => Observable.Create<TResult>(
                      observer =>
                      {
                          observer.OnNext(execute(param, result));
                          observer.OnCompleted();
                          return new CompositeDisposable();
                      }),
                  (param, result) => Observable.Create<TResult>(
                      observer =>
                      {
                          observer.OnNext(discard(param, result));
                          observer.OnCompleted();
                          return new CompositeDisposable();
                      }), context,
                  canExecute ?? Observables.True,
                  outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            HistoryContext context,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return new ReactiveCommandWithHistory<TParam, TResult>(
                commandKey, execute, discard, context,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
}