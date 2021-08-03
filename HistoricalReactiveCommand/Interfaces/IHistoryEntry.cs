using System;

namespace HistoricalReactiveCommand
{
    public interface IHistoryEntry
    {
        Action Undo { get; }
        Action Redo { get; }
    }
}