using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using HistoricalReactiveCommand.Imports;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public sealed class HistoryCommandsExecutor : ICommandExecutor, IDisposable
    {
        private readonly ICommandsManager _commandManager;
        private readonly IHistory _history;

        internal HistoryCommandsExecutor (
            ICommandsManager commandManager,
            IHistory history,
            IScheduler scheduler)
        {

            _history = history;
            _commandManager = commandManager;

            Undo = ReactiveCommand.CreateFromObservable<Unit, Unit>(unit => history.Undo(this), history.CanUndo, scheduler);
            Redo = ReactiveCommand.CreateFromObservable<Unit, Unit>(unit => history.Redo(this), history.CanRedo, scheduler);
            Clear = ReactiveCommand.CreateFromObservable(history.Clear, history.CanClear, scheduler);
        }
        
        public ReactiveCommand<Unit, Unit> Undo { get; }
        
        public ReactiveCommand<Unit, Unit> Redo { get; }
        
        public ReactiveCommand<Unit, Unit> Clear { get; }

        public void Snapshot(IHistoryEntry entry) => _history.Snapshot(entry);

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        public IObservable<Unit> Execute(IHistoryEntry entry)
        {
           return _commandManager.ResolveCommand(entry.CommandKey).Execute(entry);
        }

        public IObservable<Unit> Discard(IHistoryEntry entry)
        {
            return _commandManager.ResolveCommand(entry.CommandKey).Discard(entry);
        }
    }
}
