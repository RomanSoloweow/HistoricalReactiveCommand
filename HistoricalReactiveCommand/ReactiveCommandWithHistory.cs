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
   
    public class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveHistoryElement
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
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            _outputScheduler = outputScheduler ?? throw new ArgumentNullException(nameof(outputScheduler));
            _history = history ?? throw new ArgumentNullException(nameof(history));

            _inner = ReactiveCommand.CreateFromObservable(execute, canExecute, outputScheduler);
            _canExecuteSubscription = _canExecute.Subscribe(OnCanExecuteChanged);


            History = new ReactiveCommandHistory(history, outputScheduler);
        }
        public ReactiveCommandHistory History { get; }
        public object Parameter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string CommandKey => throw new NotImplementedException();

        public override IObservable<bool> CanExecute => _inner.CanExecute;

        public override IObservable<bool> IsExecuting => _inner.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _inner.ThrownExceptions;

        public override IObservable<TResult> Execute(TParam parameter = default) => _inner.Execute(parameter);

        public override IDisposable Subscribe(IObserver<TResult> observer) => _inner.Subscribe(observer);

        protected override void Dispose(bool disposing)
        {
            _inner.Dispose();
        }
    }
}
