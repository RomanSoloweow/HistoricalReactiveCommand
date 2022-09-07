using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using HistoricalReactiveCommand.Imports;

namespace HistoricalReactiveCommand
{
    public class History : IHistory, IDisposable
    {
        private Stack<HistoryEntry> StackRedo { get; } = new Stack<HistoryEntry>();
        private Stack<HistoryEntry> StackUndo { get; } = new Stack<HistoryEntry>();

        public HistoryEntry LastCommand()
        {
            return StackUndo.Peek();
        }

        private readonly Subject<bool> _canRecord = new Subject<bool>();
        private readonly Subject<bool> _canUndo = new Subject<bool>();
        private readonly Subject<bool> _canRedo = new Subject<bool>();
        private readonly Subject<bool> _canClear = new Subject<bool>();

        /// <summary>
        /// Время последней добавленной команды
        /// </summary>
        /// <remarks>Время именно добавленной команды, а не из стека</remarks>
        public DateTime LastCommandTime { get; set; }
        /// <summary>
        /// Можно ли добавлять вложенные команды в стек
        /// </summary>
        public bool CanAddInternalCommand { get; set; } = true;

        public History(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public IObservable<bool> CanUndo => _canUndo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanRedo => _canRedo.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanRecord => _canRecord.AsObservable().DistinctUntilChanged();
        public IObservable<bool> CanClear => _canClear.AsObservable().DistinctUntilChanged();

        public IObservable<HistoryEntry> Undo(Func<HistoryEntry, IObservable<HistoryEntry>> discard)
        {
            if (StackUndo.Count == 0)
                throw new Exception();
            
            UpdateSubjects(true);
            return discard(StackUndo.Pop()).Do(entry =>
            {
                StackRedo.Push(entry);
                UpdateSubjects();
                CanAddInternalCommand = true;
            });
        }

        public IObservable<HistoryEntry> Redo(Func<HistoryEntry, IObservable<HistoryEntry>> execute)
        {
            if (StackRedo.Count == 0)
                throw new Exception();
            
            UpdateSubjects(true);
            return execute(StackRedo.Pop()).Do(entry =>
            {
                StackUndo.Push(entry);
                UpdateSubjects();
                CanAddInternalCommand = true;
            });
        }

        public IObservable<HistoryEntry> Record(HistoryEntry entry, Func<HistoryEntry, IObservable<HistoryEntry>> execute)
        {
            StackRedo.Clear();
            UpdateSubjects(true);
            return execute(entry).Do(updatedEntry =>
            {
                StackUndo.Push(updatedEntry);
                LastCommandTime = entry.CreationtTime;
                UpdateSubjects();
            });
        }

        public IObservable<Unit> Clear()
        {
            UpdateSubjects(true);
            StackRedo.Clear();
            StackUndo.Clear();
            UpdateSubjects();
            return Observables.Unit;
        }

        public void Add(HistoryEntry entry)
        {
            StackRedo.Clear();
            UpdateSubjects(true);
            StackUndo.Push(entry);
            LastCommandTime = entry.CreationtTime;
            UpdateSubjects();
        }

        public void UndoPop()
        {
            StackUndo.Pop();
        }

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
                var hasUndoEntries = StackUndo.Any(); 
                var hasRedoEntries = StackRedo.Any();
                
                _canUndo.OnNext(hasUndoEntries);
                _canRedo.OnNext(hasRedoEntries);
                _canClear.OnNext(hasUndoEntries || hasRedoEntries);
                _canRecord.OnNext(true);
            }
        }
    }
}
