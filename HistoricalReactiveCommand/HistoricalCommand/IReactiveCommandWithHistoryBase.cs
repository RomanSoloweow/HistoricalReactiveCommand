using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistoryBase : IReactiveCommand
    {
        IObservable<IHistoryEntryBase> Execute(IHistoryEntryBase entry);
        IObservable<IHistoryEntryBase> Discard(IHistoryEntryBase entry);
    }
}