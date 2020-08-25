using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public sealed class HistoryContext : IDisposable
    {
        internal static HistoryContext GetContext(IHistory history, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }

            var context = Locator.Current.GetService<HistoryContext>(history.Id);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, history.Id);
            return context;
        }

        internal static HistoryContext GetContext(string historyId = "", IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }

            var context = Locator.Current.GetService<HistoryContext>(historyId);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext(new History(historyId), outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, historyId);
            return context;
        }

        private readonly IHistory _history;
        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            _history = history;

            var CanUndo = CanRecord
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);

            Undo = ReactiveCommand.CreateFromObservable<Unit, HistoryEntry>(
                unit => history.Undo(entry => ResolveCommand(entry).Discard(entry)),
                CanUndo, 
                outputScheduler);

            var CanRedo = CanRecord
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);

            Redo = ReactiveCommand.CreateFromObservable<Unit, HistoryEntry>(
                unit => history.Redo(entry => ResolveCommand(entry).Execute(entry)),
                CanRedo,
                outputScheduler);
            
            Clear = ReactiveCommand.CreateFromObservable(
                history.Clear, 
                history.CanClear, 
                outputScheduler);
        }

        public IObservable<bool> CanRecord => _history.CanRecord;

        public ReactiveCommand<Unit, HistoryEntry> Undo { get; }
        
        public ReactiveCommand<Unit, HistoryEntry> Redo { get; }
        
        public ReactiveCommand<Unit, Unit> Clear { get; }

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        private static IReactiveCommandWithHistory ResolveCommand(HistoryEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.CommandKey))
                throw new ArgumentException("Command key is null.", nameof(entry));
            
            var command = Locator.Current.GetService<IReactiveCommandWithHistory>(entry.CommandKey);

            if (command == null)
                throw new KeyNotFoundException($"Command with {entry.CommandKey} wasn't registered.");

            return command;
        }

        internal IObservable<HistoryEntry> Record(HistoryEntry entry, Func<HistoryEntry, IObservable<HistoryEntry>> execute) => _history.Record(entry, execute);
    }
}
