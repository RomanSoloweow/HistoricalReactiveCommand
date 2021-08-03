using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithGroupingHistoryTest
    {
        private readonly Subject<bool> _canExecuteSubject = new();
        private readonly IScheduler _scheduler = Scheduler.Immediate;

            [Fact]
            public void ShouldManageCommandHistory()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                command.CreateGrouping()
                
                command.Execute(25).Subscribe();
                Assert.Equal(25, myNumber);
                command.Execute(25).Subscribe();
                Assert.Equal(50, myNumber);
                command.Context.Undo.Execute().Subscribe();
                Assert.Equal(25, myNumber);
                command.Context.Undo.Execute().Subscribe();
                Assert.Equal(0, myNumber);
                command.Context.Redo.Execute().Subscribe();
                Assert.Equal(25, myNumber);
                command.Context.Redo.Execute().Subscribe();
                Assert.Equal(50, myNumber);
            }
    }

}