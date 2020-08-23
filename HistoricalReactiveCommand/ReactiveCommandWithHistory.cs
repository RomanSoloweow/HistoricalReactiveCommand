using ReactiveUI;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]
namespace HistoricalReactiveCommand
{
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IHistoryCommand
    {
        private readonly IDisposable _canExecuteSubscription;
        private readonly ReactiveCommand<TParam, TResult> _discard;
        private readonly ReactiveCommand<TParam, TResult> _inner;
        private readonly string _commandKey;

        internal ReactiveCommandWithHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            IHistoryCommandRegistry registry,
            IHistory history,
            string commandKey,
            IObservable<bool> canExecute,
            IScheduler outputScheduler)
        {
            _commandKey = commandKey;
            _discard = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => discard(param, (TResult) Result), 
                canExecute,
                outputScheduler);
            
            _inner = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => execute(param, (TResult) Result)
                    .Do(result => history.Snapshot(param, result, commandKey)),
                canExecute,
                outputScheduler);
            
            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
            History = new ReactiveCommandHistory<TParam, TResult>(this, registry, history, outputScheduler);
            registry.RegisterCommand(commandKey, this);
        }
        
        public ReactiveCommandHistory<TParam, TResult> History { get; }

        public override IObservable<bool> CanExecute => _inner.CanExecute;

        public override IObservable<bool> IsExecuting => _inner.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _inner.ThrownExceptions;

        public override IObservable<TResult> Execute(TParam parameter = default) => _inner.Execute(parameter);

        public override IDisposable Subscribe(IObserver<TResult> observer) => _inner.Subscribe(observer);

        protected override void Dispose(bool disposing)
        {
            _canExecuteSubscription.Dispose();
            _discard.Dispose();
            _inner.Dispose();
        }

        public object? Result { get; set; }
        
        object? IHistoryEntry.Parameter { get; }
        
        string IHistoryEntry.CommandKey => _commandKey;

        IObservable<object?> IHistoryCommand.Execute(object? parameter) => 
            _inner
                .Execute((TParam) parameter)
                .Select(result => result as object);

        IObservable<object?> IHistoryCommand.Discard(object? parameter) => 
            _discard
                .Execute((TParam) parameter)
                .Select(result => result as object);
    }
}
