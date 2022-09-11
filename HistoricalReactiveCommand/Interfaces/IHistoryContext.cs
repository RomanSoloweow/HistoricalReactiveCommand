using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IHistoryContext
    {
        IHistory History { get; }
        ReactiveCommand<Unit, Unit> Undo { get; }
        ReactiveCommand<Unit, Unit> Redo { get; }
        ReactiveCommand<Unit, Unit> Clear { get; }
        IObservable<bool> CanSnapshot { get; }
        IObservable<IHistoryEntry<TParam, TResult>> Snapshot<TParam, TResult>(IHistoryEntry<TParam, TResult> entry);
    }
}