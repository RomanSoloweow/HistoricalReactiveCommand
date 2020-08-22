using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand.History
{
    public interface IReactiveHistory
    {
        IObservable<bool> CanUndo { get; }
        IObservable<bool> IsUndoing { get; }

        IObservable<bool> CanRedo { get; }
        IObservable<bool> IsRedoing { get; }

        IObservable<bool> CanClear { get; }
        IObservable<bool> IsClearing { get; }

        ReactiveCommand<Unit, Unit> Undo { get; }
        ReactiveCommand<Unit, Unit> Redo { get; }
        ReactiveCommand<Unit, Unit> Clear { get; }

        IReactiveCommandWithHistory AddInRedo(IReactiveCommandWithHistory command);
        IReactiveCommandWithHistory AddInUndo(IReactiveCommandWithHistory command);
    }
}
