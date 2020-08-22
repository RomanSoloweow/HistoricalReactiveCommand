using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistory : IReactiveCommand
    {
        IObservable<bool> IsUndoing { get; }
        IObservable<bool> IsRedoing { get; }

        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }

        IObservable<Unit> Undo();
        IObservable<Unit> Redo();
    }
}
