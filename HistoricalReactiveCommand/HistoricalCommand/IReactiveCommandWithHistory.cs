using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistory<out TParam, out TResult> : IReactiveCommand
    {
        IObservable<IHistoryEntry<TParam, TResult>> Execute(IHistoryEntryBase entry);
        IObservable<IHistoryEntry<TParam, TResult>> Discard(IHistoryEntryBase entry);
    }
}