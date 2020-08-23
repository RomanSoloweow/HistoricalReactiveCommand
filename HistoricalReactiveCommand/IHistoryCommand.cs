using System;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IHistoryCommand : IReactiveCommand, IHistoryEntry
    {
        IObservable<object?> Execute(object? parameter);
        IObservable<object?> Discard(object? parameter);
    }
}