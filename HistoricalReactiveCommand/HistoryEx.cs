using System.Reactive.Concurrency;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public static class HistoryEx
    {
        public static ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>
            RegisterTransactionalHistory(ITransactionalHistory history,
                IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static ITransactionalHistoryContext<ITransactionalHistory, IHistoryEntry>
            RegisterTransactionalHistory(string historyId = "",
                IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static IHistoryContext<IHistory, IHistoryEntry> RegisterHistory(ITransactionalHistory history,
            IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }

        public static IHistoryContext<IHistory, IHistoryEntry> RegisterHistory(string historyId = "",
            IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
}