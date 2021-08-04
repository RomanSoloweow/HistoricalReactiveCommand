using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using HistoricalReactiveCommand.Imports;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]

namespace HistoricalReactiveCommand
{
    public static class ReactiveCommandWithTransactionalHistory
    {
        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            Func<TParam, TResult, CancellationToken, Task<TResult>> execute,
            Func<TParam, TResult, CancellationToken, Task<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromTask(execute, discard, canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            Func<TParam, TResult, Task<TResult>> execute,
            Func<TParam, TResult, Task<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable<TParam, TResult>(
                    (param, result) => Observable.StartAsync(ct => execute(param, result)),
                    (param, result) => Observable.StartAsync(ct => discard(param, result)),
                    canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            Func<TParam, Task> execute,
            Func<TParam, Task> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable<TParam, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(param)),
                (param, result) => Observable.StartAsync(ct => discard(param)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, CancellationToken, Task> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable<TParam, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(param, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistoryFromTask(
            Func<Task> execute,
            Func<Task> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable<Unit, Unit>(
                    (param, result) => Observable.StartAsync(ct => execute()),
                    (param, result) => Observable.StartAsync(ct => discard()),
                    canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistoryFromTask(
            Func<CancellationToken, Task> execute,
            Func<CancellationToken, Task> discard,
            string historyId = "",
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            return CreateWithHistoryFromObservable<Unit, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(ct)),
                (param, result) => Observable.StartAsync(ct => discard(ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
            Func<TResult, Task<TResult>> execute,
            Func<TResult, Task<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistoryFromObservable<Unit, TResult>(
                (param, result) => Observable.StartAsync(ct => execute(result)),
                (param, result) => Observable.StartAsync(ct => discard(result)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            Func<TResult, CancellationToken, Task<TResult>> execute,
            Func<TResult, CancellationToken, Task<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            return CreateWithHistoryFromObservable<Unit, TResult>(
                (param, result) => Observable.StartAsync(ct => execute(result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(result, ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistory<TParam>(
            Action<TParam> execute,
            Action<TParam> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistory<TParam, Unit>(
                (param, result) => { execute(param); return Unit.Default; },
                (param, result) => { discard(param); return Unit.Default; },
                canExecute, outputScheduler, historyId);

        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistory(
            
            Action execute,
            Action discard,
            string historyId,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }

            return CreateWithHistory<Unit, Unit>(
                    (param, result) => { execute(); return Unit.Default; },
                    (param, result) => { discard(); return Unit.Default; },
                    canExecute, outputScheduler, historyId);

        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistory<TResult>(
            
            Func<TResult, TResult> execute,
            Func<TResult, TResult> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            if (discard == null)
            {
                throw new ArgumentNullException(nameof(discard));
            }
            return CreateWithHistory<Unit, TResult>(
                (param, result) => execute(result),
                (param, result) => discard(result),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
            return new(
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
                  TransactionalHistoryContext.GetContext(historyId, outputScheduler),
                  canExecute ?? Observables.True,
                  outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            
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

            return new ReactiveCommandWithTransactionalHistory<TParam, TResult>(
                 execute, discard,
                TransactionalHistoryContext.GetContext(historyId, outputScheduler),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            Func<TParam, TResult, CancellationToken, Task<TResult>> execute,
            Func<TParam, TResult, CancellationToken, Task<TResult>> discard,
            ITransactionalHistory history,
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


            return CreateWithHistoryFromObservable<TParam, TResult>(
                (param,result) => Observable.StartAsync(ct => execute(param, result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, result, ct)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            
            Func<TParam, TResult, Task<TResult>> execute,
            Func<TParam, TResult, Task<TResult>> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<TParam, TResult>(
                    (param, result) => Observable.StartAsync(ct => execute(param, result)),
                    (param, result) => Observable.StartAsync(ct => discard(param, result)),
                     history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            
            Func<TParam, Task> execute,
            Func<TParam, Task> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<TParam, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(param)),
                (param, result) => Observable.StartAsync(ct => discard(param)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, CancellationToken, Task> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<TParam, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(param, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, ct)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistoryFromTask(
            
            Func<Task> execute,
            Func<Task> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<Unit, Unit>(
                    (param, result) => Observable.StartAsync(ct => execute()),
                    (param, result) => Observable.StartAsync(ct => discard()),
                    history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistoryFromTask(
            
            Func<CancellationToken, Task> execute,
            Func<CancellationToken, Task> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<Unit, Unit>(
                (param, result) => Observable.StartAsync(ct => execute(ct)),
                (param, result) => Observable.StartAsync(ct => discard(ct)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
            Func<TResult, Task<TResult>> execute,
            Func<TResult, Task<TResult>> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<Unit, TResult>(
                (param, result) => Observable.StartAsync(ct => execute(result)),
                (param, result) => Observable.StartAsync(ct => discard(result)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
            Func<TResult, CancellationToken, Task<TResult>> execute,
            Func<TResult, CancellationToken, Task<TResult>> discard,
            ITransactionalHistory history,
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

            return CreateWithHistoryFromObservable<Unit, TResult>(
                (param, result) => Observable.StartAsync(ct => execute(result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(result, ct)),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, Unit> CreateWithHistory<TParam>(
            
            Action<TParam> execute,
            Action<TParam> discard,
            ITransactionalHistory history,
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

            return CreateWithHistory<TParam, Unit>(
                (param, result) => { execute(param); return Unit.Default; },
                (param, result) => { discard(param); return Unit.Default; },
                history,canExecute, outputScheduler);

        }

        public static ReactiveCommandWithTransactionalHistory<Unit, Unit> CreateWithHistory(
            
            Action execute,
            Action discard,
            ITransactionalHistory history,
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

            return CreateWithHistory<Unit, Unit>(
                    (param, result) => { execute(); return Unit.Default;},
                    (param, result) => { discard(); return Unit.Default; },
                    history, canExecute, outputScheduler);

        }

        public static ReactiveCommandWithTransactionalHistory<Unit, TResult> CreateWithHistory<TResult>(
            
            Func<TResult, TResult> execute,
            Func<TResult, TResult> discard,
            ITransactionalHistory history,
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

            return CreateWithHistory<Unit, TResult>(
                (param, result)=> execute(result),
                (param, result) => discard(result),
                history, canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            ITransactionalHistory history,
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
                 execute, discard,
                TransactionalHistoryContext.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            ITransactionalHistory history,
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
                 execute, discard, 
                TransactionalHistoryContext.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            TransactionalHistoryContext context,
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

            return new ReactiveCommandWithTransactionalHistory<TParam, TResult>(
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

        public static ReactiveCommandWithTransactionalHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            TransactionalHistoryContext context,
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

            return new ReactiveCommandWithTransactionalHistory<TParam, TResult>(
                execute, discard, context,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
    
    public sealed class ReactiveCommandWithTransactionalHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
    {
        // private readonly ReactiveCommand<HistoryEntry, TResult> _discard;
        // private readonly ReactiveCommand<TParam, TResult> _execute;
        
        private readonly ReactiveCommand<TParam, TResult> _execute;
        private TParam _param;
        // private Func<TParam, TResult, IObservable<TResult>> _execute;
        // private Func<TParam, TResult, IObservable<TResult>> _discard;
        private readonly IDisposable _canExecuteSubscription;
        internal ReactiveCommandWithTransactionalHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            TransactionalHistoryContext history,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {

            if (execute == null)         throw new ArgumentNullException(nameof(execute));
            if (discard == null)         throw new ArgumentNullException(nameof(discard));
            if (canExecute == null)      throw new ArgumentNullException(nameof(canExecute));
            if (outputScheduler == null) throw new ArgumentNullException(nameof(outputScheduler));
            if (history == null)         throw new ArgumentNullException(nameof(history));
            
            History = history;
            
            var canActuallyExecute = history
                .CanSnapshot
                .CombineLatest(canExecute, (recordable, executable) => recordable && executable);
            
            _execute = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => execute?.Invoke(param, default(TResult)),
                canActuallyExecute,
                outputScheduler);
            
            _execute.Subscribe(result =>
            {
                history.Snapshot(new HistoryEntry(
                    (_)=> discard.Invoke(_param, result).Subscribe(),
                    (_)=> execute.Invoke(_param, result).Subscribe()));

            });
            
            History = history;
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
        }
        
        public TransactionalHistoryContext History {get; }

        public override IObservable<bool> CanExecute => _execute.CanExecute;

        public override IObservable<bool> IsExecuting => _execute.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _execute.ThrownExceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return _execute.Subscribe(observer);
        }

        public override IObservable<TResult> Execute(TParam parameter = default)
        {
            _param = parameter;
            return _execute.Execute(parameter);
        }

        public override IObservable<TResult> Execute()
        {
            return Execute(default!);
        }

        protected override void Dispose(bool disposing)
        {
            _canExecuteSubscription.Dispose();
            _execute.Dispose();
   
        }

    }
}
