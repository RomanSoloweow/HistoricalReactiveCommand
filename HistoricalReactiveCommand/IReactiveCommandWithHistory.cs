using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistory : IReactiveCommand
    {
        IObservable<HistoryEntry> Execute(HistoryEntry entry);
        IObservable<HistoryEntry> Discard(HistoryEntry entry);
    }
}