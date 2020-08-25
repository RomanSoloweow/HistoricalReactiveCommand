using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Splat;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class HistoryContextTests
    {
        private readonly IScheduler _scheduler = Scheduler.Immediate;

        [Fact]
        public void ShouldResolveDifferentContextsForDifferentHistories()
        {
            string historyKey = Guid.NewGuid().ToString();
            var createdContext = new HistoryContext(new History(historyKey), _scheduler);
            Locator.CurrentMutable.RegisterConstant(createdContext, historyKey);
            var context = HistoryContext.GetContext(historyKey);

            Assert.Equal(createdContext, context);

            historyKey = Guid.NewGuid().ToString();

            createdContext = new HistoryContext(new History(historyKey), _scheduler);
            Locator.CurrentMutable.RegisterConstant(createdContext, historyKey);
            var nextContext = HistoryContext.GetContext(historyKey);

            Assert.Equal(createdContext, nextContext);

            Assert.NotNull(context);
            Assert.NotNull(nextContext);
            Assert.NotEqual(context, nextContext);
        }

        [Fact]
        public void CanUndoShouldRespectHistoryState()
        {
            string historyKey = Guid.NewGuid().ToString();
            var canUndo = false;
            var canRedo = false;
            var history = new History(historyKey);


            Locator.CurrentMutable.RegisterConstant(new HistoryContext(history, _scheduler), historyKey);
            var context = HistoryContext.GetContext(historyKey);
            context.Undo.CanExecute.Subscribe(can => canUndo = can);
            context.Redo.CanExecute.Subscribe(can => canRedo = can);

            Assert.False(canUndo);
            Assert.False(canRedo);

            history.Record(
                    new HistoryEntry(Unit.Default, 42, "awesome"),
                    entry => Observable.Return(new HistoryEntry(entry.Parameter, entry.Result, entry.CommandKey)))
                .Subscribe();

            Assert.True(canUndo);
            Assert.False(canRedo);
        }
    }
}