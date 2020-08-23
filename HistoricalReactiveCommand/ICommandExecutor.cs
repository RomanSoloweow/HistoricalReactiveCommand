using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface ICommandExecutor 
    {
        IObservable<Unit> Execute(IHistoryEntry entry);
        IObservable<Unit> Discard(IHistoryEntry entry);
    }
}