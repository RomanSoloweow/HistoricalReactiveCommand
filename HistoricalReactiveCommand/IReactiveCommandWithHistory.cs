using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IReactiveCommandWithHistory : IReactiveCommand
    {
        IObservable<HistoryEntry> Execute(HistoryEntry entry);
        IObservable<HistoryEntry> Discard(HistoryEntry entry);
        void AddHistoryRecord(dynamic param);
        void MergeOrAdd(dynamic param, Func<dynamic,dynamic, bool> checkOutFunc, Func<dynamic, dynamic, dynamic> mergeFunc);
    }
}