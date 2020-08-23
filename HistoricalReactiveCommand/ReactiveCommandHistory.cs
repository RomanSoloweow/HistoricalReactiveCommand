using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public sealed class ReactiveCommandHistory<TParam, TResult> : IDisposable
    {
        private readonly ReactiveCommandWithHistory<TParam, TResult> _owner;
        private readonly IHistoryCommandRegistry _registry;
        private readonly IHistory _history;

        internal ReactiveCommandHistory(
            ReactiveCommandWithHistory<TParam, TResult> owner,
            IHistoryCommandRegistry registry,
            IHistory history,
            IScheduler scheduler)
        {
            _owner = owner;
            _history = history;
            _registry = registry;
            
            var canUndo = history.CanUndo
                .CombineLatest(owner.CanExecute, (undo, execute) => undo && execute)
                .ObserveOn(scheduler);

            var canRedo = history.CanRedo
                .CombineLatest(owner.CanExecute, (redo, execute) => redo && execute)
                .ObserveOn(scheduler);
            
            Undo = ReactiveCommand.CreateFromObservable<Unit, Unit>(unit => ApplyUndo(), canUndo, scheduler);
            Redo = ReactiveCommand.CreateFromObservable<Unit, Unit>(unit => ApplyRedo(), canRedo, scheduler);
            Clear = ReactiveCommand.Create(history.Clear, history.CanClear, scheduler);
        }
        
        public ReactiveCommand<Unit, Unit> Undo { get; }
        
        public ReactiveCommand<Unit, Unit> Redo { get; }
        
        public ReactiveCommand<Unit, Unit> Clear { get; }

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        private IObservable<Unit> ApplyUndo()
        {
            var entry = _history.Undo();
            var command = _registry.ResolveCommand(entry.CommandKey);
            return command
                .Discard(entry.Parameter)
                .Do(result => _owner.Result = command.Result)
                .Select(result => Unit.Default);
        }

        private IObservable<Unit> ApplyRedo()
        {
            var entry = _history.Redo();
            var command = _registry.ResolveCommand(entry.CommandKey);
            return command
                .Execute(entry.Parameter)
                .Do(result => _owner.Result = command.Result)
                .Select(result => Unit.Default);
        }
    }
}
