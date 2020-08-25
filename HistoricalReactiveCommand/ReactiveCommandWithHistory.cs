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
            string historyKey = "")
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
                historyKey);
        }

        public static ReactiveCommandWithHistory<TParam, TResult> CreateWithHistoryFromObservable<TParam, TResult>(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IObservable<bool>? canExecute = null,
            IScheduler? outputScheduler = null,
            string historyKey="")
        {
            return new ReactiveCommandWithHistory<TParam, TResult>(
                execute, discard,
                canExecute ?? Observables.True,
                outputScheduler ?? RxApp.MainThreadScheduler,
                History.GetContext(historyKey),
                commandKey);
        }
    }
    
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithHistory
    {
        private readonly ReactiveCommand<HistoryEntry, TResult> _discard;
        private readonly ReactiveCommand<HistoryEntry, TResult> _execute;
        private readonly IDisposable _canExecuteSubscription;
        private readonly string _commandKey;
        
        internal ReactiveCommandWithHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IObservable<bool> canExecute,
            IScheduler outputScheduler,
            HistoryContext context,
            string commandKey)
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
