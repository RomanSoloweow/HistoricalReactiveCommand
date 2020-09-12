using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.Imports;
using HistoricalReactiveCommand;
using Xunit;
using System.Threading;
using DynamicData.Annotations;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private const string CommandKey = "command-key";
        private readonly Subject<bool> _canExecuteSubject = new Subject<bool>();
        private readonly IScheduler _scheduler = Scheduler.Immediate;

        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            ICommand fixture = ReactiveCommandEx.CreateWithHistoryFromObservable<Unit, Unit>(CommandKey,
                (parameter, result) => Observables.Unit,
                (parameter, result) => Observables.Unit,
                _canExecuteSubject,
                _scheduler);

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
            int myNumber = 0;
            var command = ReactiveCommandEx.CreateWithHistory<int>("adding",
             (number) => { myNumber += number; },
             (number) => { myNumber -= number; },
              Observables.True, _scheduler);

            command.Execute(25).Subscribe();
            Assert.Equal(25, myNumber);
            command.Execute(25).Subscribe();
            Assert.Equal(50, myNumber);
            command.History.Undo.Execute().Subscribe();
            Assert.Equal(25, myNumber);
            command.History.Undo.Execute().Subscribe();
            Assert.Equal(0, myNumber);
            command.History.Redo.Execute().Subscribe();
            Assert.Equal(25, myNumber);
            command.History.Redo.Execute().Subscribe();
            Assert.Equal(50, myNumber);
        }

        [Fact]
        public async Task CanExecuteChangeOnExecutingCommandAsync()
        {

            ICommand fixture = ReactiveCommandEx.CreateWithHistoryFromObservable<Unit, Unit>(CommandKey,
              (parameter, result) => Observables.Unit,
              (parameter, result) => Observables.Unit,
              _canExecuteSubject,
              _scheduler);

            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));
            var myTask = _canExecuteSubject.FirstAsync().ToTask();
            _canExecuteSubject.OnNext(true);
            Assert.Equal(1, canExecuteChanged.Count);
            var b = await myTask;
            _canExecuteSubject.OnCompleted();
            Assert.Equal(2, canExecuteChanged.Count);    

        }
    }
}
