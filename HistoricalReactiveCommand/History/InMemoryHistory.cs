using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HistoricalReactiveCommand.History
{
    public class InMemoryHistory : IHistory, IDisposable
    {
        private readonly Subject<bool> _canUndo = new Subject<bool>();
        private readonly Subject<bool> _canRedo = new Subject<bool>();
        private readonly Subject<bool> _canClear = new Subject<bool>();

        public Stack<IHistoryEntry> StackRedo { get; } = new Stack<IHistoryEntry>();

        public Stack<IHistoryEntry> StackUndo { get; } = new Stack<IHistoryEntry>();

        public IObservable<bool> CanUndo => _canUndo.AsObservable();

        public IObservable<bool> CanRedo => _canRedo.AsObservable();

        public IObservable<bool> CanClear => _canClear.AsObservable();

        public IHistoryEntry Undo()
        {
            if (StackUndo.Count == 0)
                throw new Exception();
            
            IHistoryEntry last = StackUndo.Pop();
            StackRedo.Push(last);
            UpdateSubjects();
            return last;
        }

        public IHistoryEntry Redo()
        {
            if (StackRedo.Count == 0)
                throw new Exception();
            
            IHistoryEntry last = StackRedo.Pop();
            StackUndo.Push(last);
            UpdateSubjects();
            return last;
        }

        public void Snapshot(object parameter, object result, string commandKey)
        {
            var entry = new HistoryEntry(parameter, result, commandKey);
            StackUndo.Push(entry);
            UpdateSubjects();
        }

        public void Clear()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            UpdateSubjects();
        }

        public void Snapshot(IHistoryEntry historyElement)
        {
            StackUndo.Push(historyElement);
            UpdateSubjects();
        }

        public void Dispose()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            
            _canUndo.Dispose();
            _canRedo.Dispose();
            _canClear.Dispose();
        }

        private void UpdateSubjects()
        {
            var hasUndoEntries = StackUndo.Any(); 
            var hasRedoEntries = StackRedo.Any();
            
            _canUndo.OnNext(hasUndoEntries);
            _canRedo.OnNext(hasRedoEntries);
            _canClear.OnNext(hasUndoEntries || hasRedoEntries);
        }
        
        private sealed class HistoryEntry : IHistoryEntry
        {
            public HistoryEntry(object? parameter, object? result, string commandKey)
            {
                Parameter = parameter;
                Result = result;
                CommandKey = commandKey;
            }
            
            public object? Parameter { get; }
            public object? Result { get; }
            public string CommandKey { get; }
        }
    }
}
