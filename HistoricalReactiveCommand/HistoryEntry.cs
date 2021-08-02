using System;

namespace HistoricalReactiveCommand
{
   public delegate IObservable<HistoryEntry> HistoryActionDelegate(HistoryEntry entry);
    public class HistoryEntry
    {
        public HistoryEntry(object parameter, object result, HistoryActionDelegate undo, HistoryActionDelegate redo)
        {
            Parameter = parameter;
            Result = result;
            Undo = undo;
            Redo = redo;
        }
        
        public object Parameter { get; set; }
        public object Result { get; set; }
        
        public HistoryActionDelegate Undo { get; set; }
        public HistoryActionDelegate Redo { get; set; }
    }
}

