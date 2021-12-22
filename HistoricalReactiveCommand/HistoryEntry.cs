using System;

namespace HistoricalReactiveCommand
{
   
    public class HistoryEntry:IHistoryEntry
    {
        public HistoryEntry(Action<HistoryEntry> undo, Action<HistoryEntry> redo)
        {
            Undo = () => undo(this);
            Redo = () => redo(this);
        }
        
        public Action Undo { get; }
        public Action Redo { get; }
    }
}

