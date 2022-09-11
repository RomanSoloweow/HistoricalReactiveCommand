using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistory<TParam, TResult> : IReactiveCommandWithHistoryBase
    {
        IObservable<IHistoryEntry<TParam, TResult>> Execute(IHistoryEntry<TParam, TResult> entry);
        IObservable<IHistoryEntry<TParam, TResult>> Discard(IHistoryEntryBase entry);
    }
}