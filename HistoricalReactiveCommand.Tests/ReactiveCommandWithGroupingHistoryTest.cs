using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithGroupingHistoryTest
    {
        private readonly Subject<bool> _canExecuteSubject = new();
        private readonly IScheduler _scheduler = Scheduler.Immediate;

            [Fact]
            public void GroupingByParameterRollbackSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameter(ints =>
                {
                    ints.Sum(x => x);
                });
                
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                var canExecute = (command.History.Undo as ICommand).CanExecute(null);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.RollbackGroup();
                Assert.Equal(0, myNumber);
            }
                
            [Fact]
            public void GroupingByParameterCommitSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameter(ints =>
                {
                    ints.Sum(x => x);
                });
                    
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
            }

            [Fact]
            public void GroupingByParameterUndoRedoSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameter(ints =>
                {
                    ints.Sum(x => x);
                });
                    
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
                command.History.Undo.Execute().Subscribe();
                Assert.Equal(0, myNumber);
                command.History.Redo.Execute().Subscribe();
                Assert.Equal(100, myNumber);

            }
            
            
            [Fact]
            public void GroupingByParameterAndResultRollbackSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameterResult(args =>
                {
                    var param = args.Select(x => x.Item1).Sum(x => x);
                    return (param, Unit.Default);
                 });
                    
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.RollbackGroup();
                Assert.Equal(0, myNumber);
            }
                
            [Fact]
            public void GroupingByParameterAndResultCommitSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameterResult(args =>
                {
                    var param = args.Select(x => x.Item1).Sum(x => x);
                    return (param, Unit.Default);
;                });
                    
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
                command.History.Undo.Execute().Subscribe();
                Assert.Equal(0, myNumber);
                command.History.Redo.Execute().Subscribe();
                Assert.Equal(100, myNumber);
            }
            
            [Fact]
            public void GroupingByParameterAndResultUndoRedoSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingByParameterResult(args =>
                {
                    var param = args.Select(x => x.Item1).Sum(x => x);
                    return (param, Unit.Default);
                });
                    
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
            }
            
            [Fact]
            public void GroupingAsEntryRollbackSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

 
                var group = command.CreateGroupingAsEntry((param) =>
                {
                    var sum = param.Sum(x => x.Param);
                    return new HistoryEntry((entry) =>
                        {
                            myNumber += sum;
                        },
                        (entry) =>
                        {
                            myNumber -= sum;
                        });
                });
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.RollbackGroup();
                Assert.Equal(0, myNumber);
            }
            
            [Fact]
            public void GroupingAsEntryCommitSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingAsEntry((param) =>
                {
                    var sum = param.Sum(x => x.Param);
                    return new HistoryEntry((entry) =>
                        {
                            myNumber += sum;
                        },
                        (entry) =>
                        {
                            myNumber -= sum;
                        });
                });
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
            }
            
            [Fact]
            public void GroupingAsEntryUndoRedoSuccess()
            {
                int myNumber = 0;
                var command = ReactiveCommandWithGroupingHistory.CreateWithHistory<int>(
                    (number) => { myNumber += number; },
                    (number) => { myNumber -= number; },
                    Observables.True, _scheduler);

                var group = command.CreateGroupingAsEntry((param) =>
                {
                    var sum = param.Sum(x => x.Param);
                    return new HistoryEntry((entry) =>
                        {
                            myNumber -= sum;
                        },
                        (entry) =>
                        {
                            myNumber += sum;
                        });
                });
                command.StartGrouping(group);
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.Execute(25).Subscribe();
                command.CommitGrouping();
                Assert.Equal(100, myNumber);
                command.History.Undo.Execute().Subscribe();
                Assert.Equal(0, myNumber);
                command.History.Redo.Execute().Subscribe();
                Assert.Equal(100, myNumber);
            }
    }

}