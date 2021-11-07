using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public sealed class HistoryContext : IHistoryContext<IHistory, IHistoryEntry>, IDisposable
    {
        internal static IHistoryContext<IHistory, IHistoryEntry> GetContext(IHistory history,
            IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }

            var context = Locator.Current.GetService<IHistoryContext<IHistory, IHistoryEntry>>(history.Id);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, history.Id);
            return context;
        }

        internal static IHistoryContext<IHistory, IHistoryEntry> GetContext(string historyId = "",
            IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }

            var context = Locator.Current.GetService<IHistoryContext<IHistory, IHistoryEntry>>(historyId);

            if (context != null)
            {
                return context;
            }

            context = new HistoryContext(new History(historyId), outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, historyId);
            return context;
        }

        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            History = history;

            var canUndo = CanSnapshot
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);

            Undo = ReactiveCommand.Create(history.Undo, canUndo, outputScheduler);

            var canRedo = CanSnapshot
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);

            Redo = ReactiveCommand.Create(history.Redo, canRedo, outputScheduler);

            Clear = ReactiveCommand.Create(history.Clear, history.CanClear, outputScheduler);
        }

        public IHistory History { get; }
        public ReactiveCommand<Unit, Unit> Undo { get; }

        public ReactiveCommand<Unit, Unit> Redo { get; }

        public ReactiveCommand<Unit, Unit> Clear { get; }

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        public void Snapshot(IHistoryEntry entry) => History.Snapshot(entry);
        public IObservable<bool> CanSnapshot => History.CanSnapshot;
    }
}