using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using HistoricalReactiveCommand.Imports;

namespace HistoricalReactiveCommand
{
    public class TransactionalHistory:ITransactionalHistory
    {
        private Stack<ITransition> Transitions { get; } = new Stack<ITransition>();
        private Stack<HistoryEntry> StackRedo { get; } = new Stack<HistoryEntry>();
        private Stack<HistoryEntry> StackUndo { get; } = new Stack<HistoryEntry>();
        
        private readonly Subject<bool> _canRecord = new Subject<bool>();
        private readonly Subject<bool> _canUndo = new Subject<bool>();
        private readonly Subject<bool> _canRedo = new Subject<bool>();
        private readonly Subject<bool> _canClear = new Subject<bool>();

        public TransactionalHistory(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public IObservable<bool> CanUndo => _canUndo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanRedo => _canRedo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanRecord => _canRecord.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanClear => _canClear.AsObservable().DistinctUntilChanged();
        
        public void Undo()
        {
            if (StackUndo.Count == 0)
                throw new Exception();
            
            UpdateSubjects(true);
            var entry = StackUndo.Pop();
            entry.Undo.Invoke();
            StackRedo.Push(entry);
            UpdateSubjects();
        }
        
        public void Redo()
        {
            if (StackRedo.Count == 0)
                throw new Exception();
            
            UpdateSubjects(true);
            var entry = StackRedo.Pop();
            entry.Redo.Invoke();
            StackUndo.Push(entry);
            UpdateSubjects();
        }

        public void Snapshot(Action undo, Action redo)
        {
            StackRedo.Clear();
            UpdateSubjects(true);
            if (Transitions.Any())
            {
                Transitions.Peek().Append(undo, redo);
            }
            else
            {
                StackUndo.Push(new HistoryEntry(undo, redo));
            }
            UpdateSubjects();
        }
        

        public void Clear()
        {
            UpdateSubjects(true);
            StackRedo.Clear();
            StackUndo.Clear();
            UpdateSubjects();
        }
        
        #region ITransactionalHistory
        public void BeginTransaction(ITransition transition)
        {
            UpdateSubjects(true);
            Transitions.Push(transition);
            UpdateSubjects();
        }

        public void CommitTransaction()
        { 
            UpdateSubjects(true);
            var transition = Transitions.Pop();
            Snapshot(
                ()=>transition.Execute(this),
                ()=>transition.Discard(this));
            UpdateSubjects();
        }

        public void RollbackTransaction()
        {
            UpdateSubjects(true);
            var transition = Transitions.Pop();
            transition.Discard(this);
            UpdateSubjects();
        }
        
        #endregion ITransactionalHistory

        public void Dispose()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            
            _canUndo.Dispose();
            _canRedo.Dispose();
            _canClear.Dispose();
            _canRecord.Dispose();
        }

        
        private void UpdateSubjects(bool disableAll = false)
        {
            if (disableAll)
            {
                _canUndo.OnNext(false);
                _canRedo.OnNext(false);
                _canClear.OnNext(false);
                _canRecord.OnNext(false);
            }
            else
            {
                var inTransition = Transitions.Any();
                var hasUndoEntries = StackUndo.Any(); 
                var hasRedoEntries = StackRedo.Any();
                
                _canUndo.OnNext(hasUndoEntries && !inTransition);
                _canRedo.OnNext(hasRedoEntries && !inTransition);
                _canClear.OnNext((hasUndoEntries || hasRedoEntries )&& !inTransition);
                _canRecord.OnNext(!inTransition);
            }
        }
        
    }
}