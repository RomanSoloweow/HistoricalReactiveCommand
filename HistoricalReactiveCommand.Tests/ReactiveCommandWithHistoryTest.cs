using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private const string CommandKey = "command-key";
        private readonly Subject<bool> _canExecuteSubject = new Subject<bool>();
        private readonly IScheduler _scheduler = Scheduler.Immediate;
        private readonly IHistory _history = new History.History();
       
        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            ICommand fixture = ReactiveCommandEx.CreateWithHistoryFromObservable<Unit, Unit>(CommandKey,
                (parameter, result) => Observables.Unit,
                (parameter, result) => Observables.Unit,
                _canExecuteSubject,
                _scheduler,
                _history);

            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            _canExecuteSubject.OnNext(true);
            _canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }

        [Fact]
        public void ShouldManageCommandHistory()
        {
            var fixture = ReactiveCommandEx.CreateWithHistory<Unit, int>(CommandKey,
                (parameter, number) => number + 1,
                (parameter, number) => number - 1,
                Observables.True,
                _scheduler,
                _history);

            var latestProducedNumber = 0;
            fixture.Subscribe(number => latestProducedNumber = number);
            fixture.Execute().Subscribe();
            Assert.Equal(1, latestProducedNumber);

            fixture.History.Undo.Execute().Subscribe();
            Assert.Equal(0, latestProducedNumber);

            fixture.History.Redo.Execute().Subscribe();
            Assert.Equal(1, latestProducedNumber);
        }
    }
}
