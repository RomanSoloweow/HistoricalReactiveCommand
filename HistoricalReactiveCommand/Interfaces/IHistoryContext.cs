using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IHistoryContext<TParam, TResult>
    {
        IHistory History { get; }
        ReactiveCommand<Unit, IHistoryEntry<TParam, TResult>> Undo { get; }
        ReactiveCommand<Unit, IHistoryEntry<TParam, TResult>> Redo { get; }
        ReactiveCommand<Unit, Unit> Clear { get; }
        IObservable<bool> CanSnapshot { get; }
        IObservable<IHistoryEntry<TParam, TResult>> Snapshot(IHistoryEntry<TParam, TResult> entry);
    }
}