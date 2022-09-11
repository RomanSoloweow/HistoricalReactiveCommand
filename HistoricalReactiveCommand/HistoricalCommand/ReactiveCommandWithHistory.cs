using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Splat;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]

namespace HistoricalReactiveCommand
{
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithHistory <TParam, TResult>
    {
        private readonly ReactiveCommand<HistoryEntry<TParam, TResult>, TResult> _discard;
        private readonly ReactiveCommand<HistoryEntry<TParam, TResult>, TResult> _execute;
        private readonly IDisposable _canExecuteSubscription;
        private readonly string _commandKey;
        
        internal ReactiveCommandWithHistory(
            string commandKey,
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistoryContext context,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {

            if (execute == null)         throw new ArgumentNullException(nameof(execute));
            if (discard == null)         throw new ArgumentNullException(nameof(discard));
            if (canExecute == null)      throw new ArgumentNullException(nameof(canExecute));
            if (outputScheduler == null) throw new ArgumentNullException(nameof(outputScheduler));
            if (context == null)         throw new ArgumentNullException(nameof(context));
            if (commandKey == null)      throw new ArgumentNullException(nameof(commandKey));

            var command = Locator.Current.GetService<IReactiveCommandWithHistory<TParam, TResult>>(commandKey);
            if(command != null)    
                throw new ArgumentException($"Command with key '{commandKey}' already was registered.");
            
            History = context;

            _discard = ReactiveCommand.CreateFromObservable<HistoryEntry<TParam, TResult>, TResult>
            (
                entry => discard(entry.Parameter, entry.Result), 
                outputScheduler: outputScheduler
            );

            var canActuallyExecute = context
                .CanSnapshot
                .CombineLatest(canExecute, (recordable, executable) => recordable && executable);
            
            _execute = ReactiveCommand.CreateFromObservable<HistoryEntry<TParam, TResult>, TResult>
            (
                entry => execute(entry.Parameter, entry.Result),
                canActuallyExecute,
                outputScheduler
            );

            History = context;
            _commandKey = commandKey;
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
            
            Locator.CurrentMutable.RegisterConstant<IReactiveCommandWithHistory<TParam, TResult>>(this, commandKey);
            Locator.CurrentMutable.RegisterConstant<IReactiveCommandWithHistoryBase>(this, commandKey);
        }
        
        public IHistoryContext History {get; }

        public override IObservable<bool> CanExecute => _execute.CanExecute;

        public override IObservable<bool> IsExecuting => _execute.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _execute.ThrownExceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            return new CompositeDisposable(_execute.Subscribe(observer), _discard.Subscribe(observer));
        }

        public override IObservable<TResult> Execute(TParam parameter = default)
        {
            var historyEntry = new HistoryEntry<TParam, TResult>(_commandKey, parameter, default(TResult));
            return History.Snapshot(historyEntry).Select(entry => entry.Result);
        }

        public override IObservable<TResult> Execute()
        {
            return Execute(default(TParam));
        }

        protected override void Dispose(bool disposing)
        {
            _canExecuteSubscription.Dispose();
            _discard.Dispose();
            _execute.Dispose();
        }
        
        public IObservable<IHistoryEntry<TParam, TResult>> Execute(IHistoryEntryBase entry)
        {
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");
            
            var entry_ = entry as HistoryEntry<TParam, TResult>;
            return _execute.Execute(entry_).Select(result => { entry_.Result = result; return entry_; });
        }

        public IObservable<IHistoryEntry<TParam, TResult>> Execute(IHistoryEntry<TParam, TResult> entry)
        {
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");
            
            var entry_ = entry as HistoryEntry<TParam, TResult>;
            return _execute.Execute(entry_).Select(result => { entry_.Result = result; return entry_; });
        }

        IObservable<IHistoryEntryBase> IReactiveCommandWithHistoryBase.Discard(IHistoryEntryBase entry)
        {
            return Discard(entry);
        }

        IObservable<IHistoryEntryBase> IReactiveCommandWithHistoryBase.Execute(IHistoryEntryBase entry)
        {
            return Execute(entry);
        }

        public IObservable<IHistoryEntry<TParam, TResult>> Discard(IHistoryEntryBase entry)
        {
            if (entry.CommandKey != _commandKey)
                throw new ArgumentException($"Received {entry.CommandKey} command key instead of {_commandKey}");

            var entry_ = entry as HistoryEntry<TParam, TResult>;
            return _discard.Execute(entry_).Select(result => { entry_.Result = result; return entry_; });
        }
    }
}
