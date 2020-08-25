using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class History
    {
        public static void RegistryDefaultHistory(string historyKey="", IScheduler? outputScheduler = null)
        {
            RegistryHistory(new DefaultHistory(), historyKey, outputScheduler??RxApp.MainThreadScheduler);
        }

        public static void RegistryHistory(IHistory history, string historyKey, IScheduler? outputScheduler = null)
        {
            if (historyKey == null)
            {
                throw new ArgumentNullException(nameof(historyKey));
            }

            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            var existingContext = Locator.Current.GetService<HistoryContext>(historyKey);

            if (existingContext != null)
            {
                throw new ArgumentException($"History {historyKey} already was registered.");
            }

            var context = new HistoryContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, historyKey);
        }

        public static HistoryContext GetContext(string historyKey="")
        {
            return Locator.Current.GetService<HistoryContext>(historyKey);
        }


        public static HistoryContext GetContext(IHistory history, string historyKey, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            if (historyKey == null)
            {
                throw new ArgumentNullException(nameof(historyKey));
            }

            var context = GetContext(historyKey);

            if(context!=null)
            {
                return context;
            }

            RegistryHistory(history, historyKey, outputScheduler);

            return GetContext(historyKey);
        }
    }
    
    public sealed class HistoryContext : IDisposable
    {
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
