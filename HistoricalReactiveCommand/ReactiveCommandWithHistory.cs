using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using HistoricalReactiveCommand.History;
using HistoricalReactiveCommand.Imports;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]

namespace HistoricalReactiveCommand
{
    public class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithHistory
    {
        private readonly IDisposable _canExecuteSubscription;
        private readonly Func<TParam, IObservable<TResult>> _execute;
        private readonly ReactiveCommand<TParam, TResult> _inner;
        private readonly IObservable<bool> _canExecute;
        private readonly IScheduler _outputScheduler;
        private readonly IReactiveHistory _history;

        internal ReactiveCommandWithHistory(
            Func<TParam, IObservable<TResult>>? execute,
            IObservable<bool>? canExecute,
            IScheduler? outputScheduler,
            IReactiveHistory? history)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            _outputScheduler = outputScheduler ?? throw new ArgumentNullException(nameof(outputScheduler));
            _history = history ?? throw new ArgumentNullException(nameof(history));
            
            _inner = ReactiveCommand.CreateFromObservable(execute, canExecute, outputScheduler);
            _canExecuteSubscription = _canExecute.Subscribe(OnCanExecuteChanged);
        }
        
        public override IDisposable Subscribe(IObserver<TResult> observer) => _inner.Subscribe(observer);

        public override IObservable<TResult> Execute(TParam parameter = default) => _inner.Execute(parameter);

        public override IObservable<bool> CanExecute => _inner.CanExecute;

        public override IObservable<bool> IsExecuting => _inner.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _inner.ThrownExceptions;
        
        public IObservable<bool> IsUndoing => Observables.False;
        
        public IObservable<bool> IsRedoing => Observables.False;
        
        public IObservable<bool> CanUndo => Observables.False;
        
        public IObservable<bool> CanRedo => Observables.False;
        
        public IObservable<Unit> Undo() => Observables.Unit;

        public IObservable<Unit> Redo() => Observables.Unit;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _canExecuteSubscription.Dispose();
            }
        }
    }
}
