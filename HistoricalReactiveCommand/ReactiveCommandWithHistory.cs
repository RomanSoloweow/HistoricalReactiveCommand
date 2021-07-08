using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using HistoricalReactiveCommand.Imports;
using Splat;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Resources;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]

namespace HistoricalReactiveCommand
{
    public static class ReactiveCommandEx
    {
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            string commandKey,
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

            return CreateWithHistoryFromTask(commandKey,
                execute, discard, canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromTask<TParam, TResult>(
            string commandKey,
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

            return CreateWithHistoryFromObservable<TParam, TResult>(commandKey,
                    (param, result) => Observable.StartAsync(ct => execute(param, result)),
                    (param, result) => Observable.StartAsync(ct => discard(param, result)),
                    canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
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

            return CreateWithHistoryFromObservable<TParam, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(param)),
                (param, result) => Observable.StartAsync(ct => discard(param)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistoryFromTask<TParam>(
            string commandKey,
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

            return CreateWithHistoryFromObservable<TParam, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(param, ct)),
                (param, result) => Observable.StartAsync(ct => discard(param, ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask<TResult>(
            string commandKey,
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

            return CreateWithHistoryFromObservable<Unit, Unit>(commandKey,
                    (param, result) => Observable.StartAsync(ct => execute()),
                    (param, result) => Observable.StartAsync(ct => discard()),
                    canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask<TResult>(
            string commandKey,
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
            return CreateWithHistoryFromObservable<Unit, Unit>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(ct)),
                (param, result) => Observable.StartAsync(ct => discard(ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
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

            return CreateWithHistoryFromObservable<Unit, TResult>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(result)),
                (param, result) => Observable.StartAsync(ct => discard(result)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistoryFromTask<TResult>(
            string commandKey,
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
            return CreateWithHistoryFromObservable<Unit, TResult>(commandKey,
                (param, result) => Observable.StartAsync(ct => execute(result, ct)),
                (param, result) => Observable.StartAsync(ct => discard(result, ct)),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<TParam, Unit> CreateWithHistory<TParam>(
            string commandKey,
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

            return CreateWithHistory<TParam, Unit>(commandKey,
                (param, result) => { execute(param); return Unit.Default; },
                (param, result) => { discard(param); return Unit.Default; },
                canExecute, outputScheduler, historyId);

        }

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory<TResult>(
            string commandKey,
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

            return CreateWithHistory<Unit, Unit>(commandKey,
                    (param, result) => { execute(); return Unit.Default; },
                    (param, result) => { discard(); return Unit.Default; },
                    canExecute, outputScheduler, historyId);

        }

        public static ReactiveCommandWithHistory<Unit, TResult> CreateWithHistory<TResult>(
            string commandKey,
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
            return CreateWithHistory<Unit, TResult>(commandKey,
                (param, result) => execute(result),
                (param, result) => discard(result),
                canExecute, outputScheduler, historyId);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyId = "")
        {
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
                  HistoryContext.GetContext(historyId, outputScheduler),
                  canExecute ?? Observables.True,
                  outputScheduler ?? RxApp.MainThreadScheduler);
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

            return new ReactiveCommandWithHistory<TParam, TResult>(
                commandKey, execute, discard,
                HistoryContext.GetContext(historyId, outputScheduler),
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler);
        }

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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask<TResult>(
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistoryFromTask<TResult>(
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

        public static ReactiveCommandWithHistory<Unit, Unit> CreateWithHistory<TResult>(
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
                HistoryContext.GetContext(history, outputScheduler),
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
                HistoryContext.GetContext(history, outputScheduler),
                canExecute, outputScheduler);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
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
    
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithHistory
    {
        private readonly ReactiveCommand<HistoryEntry, TResult> _discard;
        private readonly ReactiveCommand<HistoryEntry, TResult> _execute;
        private readonly IDisposable _canExecuteSubscription;
        private readonly string _commandKey;
        
        internal ReactiveCommandWithHistory(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            HistoryContext context,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {

            if (execute == null)         throw new ArgumentNullException(nameof(execute));
            if (discard == null)         throw new ArgumentNullException(nameof(discard));
            if (canExecute == null)      throw new ArgumentNullException(nameof(canExecute));
            if (outputScheduler == null) throw new ArgumentNullException(nameof(outputScheduler));
            if (context == null)         throw new ArgumentNullException(nameof(context));
            if (commandKey == null)      throw new ArgumentNullException(nameof(commandKey));

            History = context;

            _discard = ReactiveCommand.CreateFromObservable<HistoryEntry, TResult>(
                entry => discard((TParam) entry.Parameter, (TResult) entry.Result), 
                outputScheduler: outputScheduler);

            var canActuallyExecute = context
                .CanRecord
                .CombineLatest(canExecute, (recordable, executable) => recordable && executable);
            
            _execute = ReactiveCommand.CreateFromObservable<HistoryEntry, TResult>(
                entry => execute((TParam) entry.Parameter, (TResult) entry.Result),
                canActuallyExecute,
                outputScheduler);

            History = context;
            _commandKey = commandKey;
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
            
      

            Locator.CurrentMutable.RegisterConstant<IReactiveCommandWithHistory>(this, commandKey);
        }
        
        public HistoryContext History {get; }

        public override IObservable<bool> CanExecute => _execute.CanExecute;

        public override IObservable<bool> IsExecuting => _execute.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _execute.ThrownExceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return new CompositeDisposable(_execute.Subscribe(observer), _discard.Subscribe(observer));
        }

        public override IObservable<TResult> Execute(TParam parameter = default)
        {
            var historyEntry = new HistoryEntry(parameter, default(TResult), _commandKey);
            var withHistory = this as IReactiveCommandWithHistory;
            return History
                .Record(historyEntry, entry => withHistory.Execute(entry))
                .Select(entry => (TResult) entry.Result);
        }

        public override IObservable<TResult> Execute()
        {
            return Execute(default!);
        }

        protected override void Dispose(bool disposing)
        {
            _canExecuteSubscription.Dispose();
            _discard.Dispose();
            _execute.Dispose();
        }

        IObservable<HistoryEntry> IReactiveCommandWithHistory.Execute(HistoryEntry entry)
        {   
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");

            return _execute.Execute(entry).Select(result => { entry.Result = result; return entry; });
        }

        IObservable<HistoryEntry> IReactiveCommandWithHistory.Discard(HistoryEntry entry)
        {
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");

            return _discard.Execute(entry).Select(result => { entry.Result = result; return entry; });
        }
    }
}
