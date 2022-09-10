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
    public static class ReactiveCommandWithHistory
    {
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
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

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistory<TParam>(
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory(
            
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistory<TResult>(
            
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

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
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
                  HistoryContext.GetContext<TParam, TResult>(historyId, outputScheduler),
                  canExecute ?? Observables.True,
                  outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            
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

            return new ReactiveCommandWithHistory<TParam, TResult>(
                 execute, discard,
                HistoryContext.GetContext<TParam, TResult>(historyId, outputScheduler),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            Func<TParam, TResult, CancellationToken, Task<TResult>> execute,
            Func<TParam, TResult, CancellationToken, Task<TResult>> discard,
            IHistory<TParam, TResult> history,
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

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            
            Func<TParam, TResult, Task<TResult>> execute,
            Func<TParam, TResult, Task<TResult>> discard,
            IHistory<TParam, TResult> history,
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            
            Func<TParam, Task> execute,
            Func<TParam, Task> discard,
            IHistory<TParam, Unit> history,
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, CancellationToken, Task> discard,
            IHistory<TParam, Unit> history,
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            
            Func<Task> execute,
            Func<Task> discard,
            IHistory<Unit, Unit> history,
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask(
            
            Func<CancellationToken, Task> execute,
            Func<CancellationToken, Task> discard,
            IHistory<Unit, Unit> history,
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
            Func<TResult, Task<TResult>> execute,
            Func<TResult, Task<TResult>> discard,
            IHistory<Unit, TResult> history,
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            
            Func<TResult, CancellationToken, Task<TResult>> execute,
            Func<TResult, CancellationToken, Task<TResult>> discard,
            IHistory<Unit, TResult> history,
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

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistory<TParam>(
            
            Action<TParam> execute,
            Action<TParam> discard,
            IHistory<TParam, Unit> history,
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory(
            
            Action execute,
            Action discard,
            IHistory<Unit, Unit> history,
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

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistory<TResult>(
            
            Func<TResult, TResult> execute,
            Func<TResult, TResult> discard,
            IHistory<Unit, TResult> history,
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

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IHistory<TParam, TResult> history,
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
                HistoryContext.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistory<TParam, TResult> history,
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
                HistoryContext.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> context,
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
            
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> context,
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
                execute, discard, context,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
    
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
    {
        private readonly ReactiveCommand<TParam, TResult> _execute;
        private TParam _param;
        private readonly IDisposable _canExecuteSubscription;
        internal Func<TParam, TResult, IObservable<TResult>> executeAction;
        internal Func<TParam, TResult, IObservable<TResult>> discardAction;
        internal ReactiveCommandWithHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> context,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {

            if (execute == null)         throw new ArgumentNullException(nameof(execute));
            if (discard == null)         throw new ArgumentNullException(nameof(discard));
            if (canExecute == null)      throw new ArgumentNullException(nameof(canExecute));
            if (outputScheduler == null) throw new ArgumentNullException(nameof(outputScheduler));
            if (context == null)         throw new ArgumentNullException(nameof(context));
            
            History = context;
            executeAction = execute;
            discardAction = discard;
            
            var canActuallyExecute = context
                .CanSnapshot
                .CombineLatest(canExecute, (recordable, executable) => recordable && executable);
            
            _execute = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => execute?.Invoke(param, default(TResult)),
                canActuallyExecute,
                outputScheduler);
            
            _execute.Subscribe(result =>
            {
                context.Snapshot(CreateHistoryEntry(_param, result));

            });
            
            History = context;
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
        }
        
        public IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> History {get; }

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
        
        public HistoryEntry<TParam, TResult> CreateHistoryEntry(TParam param, TResult result)
        {
            return new HistoryEntry<TParam, TResult>(
                (_) => discardAction.Invoke(param, result).Subscribe(),
                (_) => executeAction.Invoke(param, result).Subscribe(),
                param, result);
        }
        
    }
}
