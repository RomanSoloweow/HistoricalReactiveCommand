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
        ReactiveCommand<Unit, Unit> UndoCommand { get; }
        ReactiveCommand<Unit, Unit> RedoCommand { get; }
        ReactiveCommand<Unit, Unit> ClearHistory { get; }
    }
}
