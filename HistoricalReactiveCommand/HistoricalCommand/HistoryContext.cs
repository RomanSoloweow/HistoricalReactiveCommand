using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using HistoricalReactiveCommand.Imports;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class HistoryContext
    {
        internal static HistoryContext<TParam, TResult> GetContext<TParam, TResult> (IHistory history, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }

            var context = Locator.Current.GetService<HistoryContext<TParam, TResult>>(history.Id);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext<TParam, TResult>(history, outputScheduler ?? RxApp.MainThreadScheduler);
            
            Locator.CurrentMutable.RegisterConstant(context, history.Id);
            return context;
        }

        internal static HistoryContext<TParam, TResult> GetContext<TParam, TResult> (string historyId = "", IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }

            var context = Locator.Current.GetService<HistoryContext<TParam, TResult>>(historyId);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext<TParam, TResult>(new History(historyId), outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, historyId);
            return context;
        }
    }
    
    public sealed class HistoryContext<TParam, TResult> : IHistoryContext<TParam, TResult>, IDisposable
    {
        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            History = history;
            
            var CanUndo = history.CanRecord
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);
            
            Undo = ReactiveCommand.CreateFromObservable<Unit, IHistoryEntry<TParam, TResult>>(
                unit => history.Undo(entry => ResolveCommand(entry).Discard(entry)),
                CanUndo, 
                outputScheduler);
            
            var CanRedo = history.CanRecord
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);
            
            Redo = ReactiveCommand.CreateFromObservable<Unit, IHistoryEntry<TParam, TResult>>(
                unit => history.Redo(entry => ResolveCommand(entry).Execute(entry)),
                 CanRedo, outputScheduler);
            
            Clear = ReactiveCommand.CreateFromObservable(
                history.Clear, 
                history.CanClear, 
                outputScheduler);
        }
        
        private static IReactiveCommandWithHistory<TParam, TResult> ResolveCommand(IHistoryEntryBase entry)
        {
            if (string.IsNullOrWhiteSpace(entry.CommandKey))
                throw new ArgumentException("Command key is null.", nameof(entry));
            
            var command = Locator.Current.GetService<IReactiveCommandWithHistory<TParam, TResult>>(entry.CommandKey);
        
            if (command == null)
                throw new KeyNotFoundException($"Command with key '{entry.CommandKey}' wasn't registered.");
        
            return command;
        }
        
        public IHistory History { get; }
        public IObservable<bool> CanSnapshot => History.CanRecord;
        public ReactiveCommand<Unit, IHistoryEntry<TParam, TResult>> Undo { get; }
        public ReactiveCommand<Unit, IHistoryEntry<TParam, TResult>> Redo { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        
        public IObservable<IHistoryEntry<TParam, TResult>> Snapshot(IHistoryEntry<TParam, TResult> entry)
        {
            var command = ResolveCommand(entry);
            return History.Record(entry, command.Execute);
        }

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }
    }
}
