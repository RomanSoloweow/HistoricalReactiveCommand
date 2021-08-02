using System;

namespace HistoricalReactiveCommand
{
   public delegate IObservable<HistoryEntry> HistoryActionDelegate(HistoryEntry entry);
    public class HistoryEntry
    {
        public HistoryEntry(Action undo, Action redo)
        {
            Undo = undo;
            Redo = redo;
        }
        
        public Action Undo { get; set; }
        public Action Redo { get; set; }
    }
}

