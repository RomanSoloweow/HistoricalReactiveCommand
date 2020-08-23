using HistoricalReactiveCommand.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HistoricalReactiveCommand.History
{
    public class ReactiveHistory : IHistory, IDisposable
    {
        private readonly Subject<bool> _canUndo = new Subject<bool>();
        private readonly Subject<bool> _canRedo = new Subject<bool>();
        private readonly Subject<bool> _canClear = new Subject<bool>();

        public Stack<IHistoryEntry> StackRedo { get; } = new Stack<IHistoryEntry>();

        public Stack<IHistoryEntry> StackUndo { get; } = new Stack<IHistoryEntry>();

        public IObservable<bool> CanUndo => _canUndo.AsObservable();

        public IObservable<bool> CanRedo => _canRedo.AsObservable();

        public IObservable<bool> CanClear => _canClear.AsObservable();

        public IObservable<Unit> Undo(ICommandExecutor executor)
        {
            if (StackUndo.Count == 0)
                throw new Exception();
            
            IHistoryEntry entry = StackUndo.Pop();
            var observable = executor.Discard(entry);
            StackRedo.Push(entry);
            UpdateSubjects();
            return observable;
        }

        public IObservable<Unit> Redo(ICommandExecutor executor)
        {
            if (StackRedo.Count == 0)
                throw new Exception();
            
            IHistoryEntry entry = StackRedo.Pop();
            var observable = executor.Execute(entry);
            StackUndo.Push(entry);
            UpdateSubjects();
            return observable;
        }

        public IObservable<Unit> Clear()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            UpdateSubjects();
            return Observables.Unit;
        }

        public void Snapshot(IHistoryEntry historyElement)
        {
            StackRedo.Clear();
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
       
    }
}
