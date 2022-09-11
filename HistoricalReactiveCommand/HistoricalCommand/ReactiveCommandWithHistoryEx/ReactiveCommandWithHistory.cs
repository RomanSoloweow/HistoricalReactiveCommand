using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using HistoricalReactiveCommand.Imports;

namespace HistoricalReactiveCommand
{
    public static partial class ReactiveCommandWithHistoryEx
    {
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            return CreateWithHistory
            (
                commandKey, execute, discard,
                HistoryContextEx.GetContext(historyId, outputScheduler),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler
            );
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable
            (
                commandKey, execute, discard,
                HistoryContextEx.GetContext(historyId, outputScheduler),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler
            );
        }
        
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
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
                      }),
                  historyContext,
                  canExecute ?? Observables.True,
                  outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistoryContext historyContext,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (historyContext == null)
            {
                throw new ArgumentNullException(nameof(historyContext));
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
                commandKey, execute, discard,
                historyContext,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
}