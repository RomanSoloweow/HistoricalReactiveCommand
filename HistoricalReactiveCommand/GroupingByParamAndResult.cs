using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalReactiveCommand
{
    public class GroupingByParamAndResult<TParam, TResult>:IGrouping<TParam, TResult>
    {
        private Func<TParam, TResult, IObservable<TResult>> _execute;
        private Func<TParam, TResult, IObservable<TResult>> _discard;
        private Func<List<(TParam, TResult)>, (TParam, TResult)> _groupingAction;
        public GroupingByParamAndResult(
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard, 
            Func<List<(TParam, TResult)>, (TParam, TResult)> groupingAction)
        {
            _execute = execute;
            _discard = discard;
            _groupingAction = groupingAction;
        }
     
        private List<IHistoryEntryForGroup<TParam, TResult>> Groups = new();
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry)
        {
            Groups.Add(entry);
        }

        public IHistoryEntry Group()
        {
            var parameters = Groups.Select(
                x => (x.Param, x.Result)).ToList();
            var (param, result) = _groupingAction(parameters);
            return new HistoryEntry(
                (entry) => { _discard(param, result).Subscribe();},
                (entry) => { _execute(param, result).Subscribe();});
        }

        public void Rollback()
        {
            var parameters = Groups.Select(
                x => (x.Param, x.Result)).ToList();
            var (param, result) = _groupingAction(parameters);

            _discard(param, result).Subscribe();
        }
    }
}