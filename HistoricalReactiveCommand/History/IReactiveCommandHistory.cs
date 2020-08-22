using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Text;

namespace HistoricalReactiveCommand.History
{
    public class ReactiveCommandHistory
    {
        public ReactiveCommandHistory(IReactiveHistory reactiveHistory, IScheduler? outputScheduler)
        {
            UndoCommand = ReactiveCommand.CreateFromObservable(reactiveHistory.Undo, reactiveHistory.CanUndo, outputScheduler);
            RedoCommand = ReactiveCommand.CreateFromObservable(reactiveHistory.Redo, reactiveHistory.CanRedo, outputScheduler);
            ClearHistory = ReactiveCommand.CreateFromObservable(reactiveHistory.Clear, reactiveHistory.CanClear, outputScheduler);
        }
        public ReactiveCommand<Unit, Unit> UndoCommand { get; }
        public ReactiveCommand<Unit, Unit> RedoCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearHistory { get; }
    }
}
