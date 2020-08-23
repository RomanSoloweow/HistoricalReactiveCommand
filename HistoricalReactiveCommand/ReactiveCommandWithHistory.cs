using HistoricalReactiveCommand.History;
using ReactiveUI;
using SimpleStateMachineNodeEditor.Helpers.Extensions;
using System;
using System.ComponentModel.Design;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HistoricalReactiveCommand.Tests")]
namespace HistoricalReactiveCommand
{
    public sealed class ReactiveCommandWithHistory<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, ICommandExecutor
    {
        private readonly IDisposable _canExecuteSubscription;
        private readonly ReactiveCommand<TParam, TResult> _discard;
        private readonly ReactiveCommand<TParam, TResult> _inner;
        private readonly string _commandKey;
        private TResult _result;
        private TParam _prameter;

        internal ReactiveCommandWithHistory(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard,
            HistoryCommandsExecutor  commandsHistory,
            string commandKey,
            IObservable<bool> canExecute,
            IObservable<bool> canDiscard,
            IScheduler outputScheduler)
        {
            _commandKey = commandKey;
            _discard = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => discard(param, _result), 
                canExecute,
                outputScheduler);

            _inner = ReactiveCommand.CreateFromObservable<TParam, TResult>(
                param => execute(param, _result),
                canDiscard,
                outputScheduler);

            _canExecuteSubscription = canExecute.Subscribe(OnCanExecuteChanged);
            HistoryCommands = commandsHistory;

        }
        
        public HistoryCommandsExecutor HistoryCommands {get; }

        public override IObservable<bool> CanExecute => _inner.CanExecute;

        public override IObservable<bool> IsExecuting => _inner.IsExecuting;

        public override IObservable<Exception> ThrownExceptions => _inner.ThrownExceptions;

        public override IDisposable Subscribe(IObserver<TResult> observer) => _inner.Subscribe(observer);

        protected override void Dispose(bool disposing)
        {
            _canExecuteSubscription.Dispose();
            _discard.Dispose();
            _inner.Dispose();
            
        }

        public override IObservable<TResult> Execute(TParam parameter = default)
        {
            //Invoke inner and HistoryCommands.Snapshot(ToSnapShot())
            throw new NotImplementedException();
        }
        IObservable<Unit> ICommandExecutor.Execute(IHistoryEntry entry)
        {
            //Invoke this  by inner
            //element.Result = _unExecute(element.Parameter.Cast<TParameter>(), element.Result.Cast<TResult>());
            throw new NotImplementedException();
        }

        IObservable<Unit> ICommandExecutor.Discard(IHistoryEntry entry)
        {
            //Invoke this  by _discard
            //element.Result = _unExecute(element.Parameter.Cast<TParameter>(), element.Result.Cast<TResult>());
            throw new NotImplementedException();

        }

        private IHistoryEntry ToSnapShot()
        {
            return new HistoryEntry(_commandKey, _prameter, _result);
        }


        //IObservable<object?> IHistoryCommand.Execute(object? parameter) =>
        //    _inner
        //        .Execute((TParam)parameter)
        //        .Select(result => result as object);

        //IObservable<object?> IHistoryCommand.Discard(object? parameter) =>
        //    _discard
        //        .Execute((TParam)parameter)
        //        .Select(result => result as object);
    }
}
