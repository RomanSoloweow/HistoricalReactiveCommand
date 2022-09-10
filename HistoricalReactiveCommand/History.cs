﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HistoricalReactiveCommand
{
    public class History<TParam, TResult> : IHistory<TParam, TResult>
    {
        private Stack<IHistoryEntry<TParam, TResult>> StackRedo { get; } = new();
        private Stack<IHistoryEntry<TParam, TResult>> StackUndo { get; } = new();
        
        private readonly Subject<bool> _canSnapshot = new();
        private readonly Subject<bool> _canUndo = new();
        private readonly Subject<bool> _canRedo = new();
        private readonly Subject<bool> _canClear = new();

        public History(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public IObservable<bool> CanUndo => _canUndo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanRedo => _canRedo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanSnapshot => _canSnapshot.AsObservable().DistinctUntilChanged();
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
        
        public void Snapshot(IHistoryEntry<TParam, TResult> entry)
        {
            StackRedo.Clear();
            UpdateSubjects(true);
            StackUndo.Push(entry);
            UpdateSubjects();
        }

        public void Clear()
        {
            UpdateSubjects(true);
            StackRedo.Clear();
            StackUndo.Clear();
            UpdateSubjects();
        }

        public void Dispose()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            
            _canUndo.Dispose();
            _canRedo.Dispose();
            _canClear.Dispose();
            _canSnapshot.Dispose();
        }

        private void UpdateSubjects(bool disableAll = false)
        {
            if (disableAll)
            {
                _canUndo.OnNext(false);
                _canRedo.OnNext(false);
                _canClear.OnNext(false);
                _canSnapshot.OnNext(false);
            }
            else
            {
                var hasUndoEntries = StackUndo.Any(); 
                var hasRedoEntries = StackRedo.Any();
                
                _canUndo.OnNext(hasUndoEntries);
                _canRedo.OnNext(hasRedoEntries);
                _canClear.OnNext(hasUndoEntries || hasRedoEntries);
                _canSnapshot.OnNext(true);
            }
        }
    }
}
