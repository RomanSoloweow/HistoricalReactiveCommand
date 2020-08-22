using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.History;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private readonly IReactiveHistory _dummyHistory = new DummyReactiveHistory();

        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            Subject<bool> canExecuteSubject = new Subject<bool>();
            ICommand fixture = new ReactiveCommandWithHistory<Unit, Unit>(
                unit => Observables.Unit,
                canExecuteSubject,
                Scheduler.Immediate,
                _dummyHistory);

            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }
    }
}
