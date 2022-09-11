using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class HistoryContextEx
    {
        internal static HistoryContext GetContext (IHistory history, IScheduler? outputScheduler = null)
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

        internal static HistoryContext GetContext (string historyId = "", IScheduler? outputScheduler = null)
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
    }
    
    public sealed class HistoryContext : IHistoryContext, IDisposable
    {
        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            History = history;
            
            var canUndo = history.CanRecord
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);
            
            Undo = ReactiveCommand.CreateFromObservable<Unit, Unit>(
                unit => history.Undo(entry => ResolveCommand(entry).Discard(entry)).Select(_ => Unit.Default),
                canUndo, 
                outputScheduler);
            
            var canRedo = history.CanRecord
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);
            
            Redo = ReactiveCommand.CreateFromObservable<Unit, Unit>(
                unit => history.Redo(entry => ResolveCommand(entry).Execute(entry)).Select(_ => Unit.Default),
                 canRedo, outputScheduler);
            
            Clear = ReactiveCommand.CreateFromObservable(
                history.Clear, 
                history.CanClear, 
                outputScheduler);
        }
        
        private static IReactiveCommandWithHistory<TParam, TResult> ResolveCommand<TParam, TResult>(IHistoryEntry<TParam, TResult> entry)
        {
            if (string.IsNullOrWhiteSpace(entry.CommandKey))
                throw new ArgumentException("Command key is null.", nameof(entry));
            
            var command = Locator.Current.GetService<IReactiveCommandWithHistory<TParam, TResult>>(entry.CommandKey);
        
            if (command == null)
                throw new KeyNotFoundException($"Command with key '{entry.CommandKey}' wasn't registered.");
        
            return command;
        }
        
        private static IReactiveCommandWithHistoryBase ResolveCommand(IHistoryEntryBase entry)
        {
            if (string.IsNullOrWhiteSpace(entry.CommandKey))
                throw new ArgumentException("Command key is null.", nameof(entry));
            
            var command = Locator.Current.GetService<IReactiveCommandWithHistoryBase>(entry.CommandKey);
        
            if (command == null)
                throw new KeyNotFoundException($"Command with key '{entry.CommandKey}' wasn't registered.");
        
            return command;
        }
        
        public IHistory History { get; }
        public IObservable<bool> CanSnapshot => History.CanRecord;

        public ReactiveCommand<Unit, Unit> Undo { get; }
        public ReactiveCommand<Unit, Unit> Redo { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        
        public IObservable<IHistoryEntry<TParam, TResult>> Snapshot<TParam, TResult>(IHistoryEntry<TParam, TResult> entry)
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
