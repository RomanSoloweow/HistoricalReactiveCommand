using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public class TransactionalHistoryContext : ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>
    {
        internal static ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry> GetContext(
            ITransactionalHistory history, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }

            var context =
                Locator.Current.GetService<ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>>(
                    history.Id);

            if (context != null)
            {
                return context;
            }

            context = new TransactionalHistoryContext(history, outputScheduler ?? RxApp.MainThreadScheduler);

            Locator.CurrentMutable.RegisterConstant(context,
                typeof(ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>), history.Id);
            Locator.CurrentMutable.RegisterConstant(context, typeof(IHistoryContext<IHistory, IHistoryEntry>),
                history.Id);

            return context;
        }

        internal static ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry> GetContext(
            string historyId = "", IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }

            var context =
                Locator.Current.GetService<ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>>(
                    historyId);

            if (context != null)
            {
                return context;
            }

            context = new TransactionalHistoryContext(new TransactionalHistory(historyId),
                outputScheduler ?? RxApp.MainThreadScheduler);

            Locator.CurrentMutable.RegisterConstant(context,
                typeof(ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>), historyId);
            Locator.CurrentMutable.RegisterConstant(context, typeof(IHistoryContext<IHistory, IHistoryEntry>),
                historyId);
            return context;
        }

        internal TransactionalHistoryContext(ITransactionalHistory history, IScheduler outputScheduler)
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

        public ITransactionalHistory History { get; }
        public ReactiveCommand<Unit, Unit> Undo { get; }
        public ReactiveCommand<Unit, Unit> Redo { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        public IObservable<bool> CanSnapshot => History.CanSnapshot;
        public void Snapshot(IHistoryEntry entry) => History.Snapshot(entry);
        public void BeginTransaction(ITransition transition) => History.BeginTransaction(transition);
        public void CommitTransaction() => History.CommitTransaction();
        public void RollbackTransaction() => History.RollbackTransaction();

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }
    }
}