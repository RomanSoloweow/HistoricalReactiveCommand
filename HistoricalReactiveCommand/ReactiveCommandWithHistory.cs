using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using HistoricalReactiveCommand.Imports;
using Splat;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]

namespace HistoricalReactiveCommand
{
    public static class ReactiveCommandEx
    {
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistory<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, TResult> execute,
            Func<TParam, TResult, TResult> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            IHistory? history = null)
        {
            return CreateWithHistoryFromObservable<TParam, TResult>(
                commandKey,
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
                canExecute,
                outputScheduler,
                history);
        }
        
        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            IHistory? history = null)
        {
            if (commandKey == null) throw new ArgumentNullException(nameof(commandKey));
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (discard == null) throw new ArgumentNullException(nameof(discard));

            var command = new ReactiveCommandWithHistory<TParam, TResult>(
                execute, discard,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler,
                History.GetContext(history, outputScheduler),
                history ?? Locator.Current.GetService<IHistory>(),
                commandKey);
            
            Locator.CurrentMutable.RegisterConstant(command, typeof(IReactiveCommandWithHistory), commandKey);
            return command;
        }
    }
    
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithHistory
    {
        private readonly ReactiveCommand<HistoryEntry, TResult> _discard;
        private readonly ReactiveCommand<HistoryEntry, TResult> _execute;
        private readonly IDisposable _canExecuteSubscription;
        private readonly IHistory _history;
        private readonly string _commandKey;
        
        internal ReactiveCommandWithHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IObservable<bool> canExecute,
            IScheduler outputScheduler,
            HistoryContext context,
            IHistory history,
            string commandKey)
        {
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

            _history = history;
            _commandKey = commandKey;
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
            
            History = context;
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
            return _history
                .Record(historyEntry, entry => withHistory.Execute(entry))
                .Select(entry => (TResult) entry.Result);
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
            return _execute.Execute(entry).Select(result => new HistoryEntry(entry.Parameter, result, entry.CommandKey));
        }

        IObservable<HistoryEntry> IReactiveCommandWithHistory.Discard(HistoryEntry entry)
        {
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");
            return _discard.Execute(entry).Select(result => new HistoryEntry(entry.Parameter, result, entry.CommandKey));
        }
    }
}
