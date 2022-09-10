using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalReactiveCommand
{
    public class GroupingByParam<TParam, TResult> : IGrouping<TParam, TResult>
    {
        private readonly Func<TParam, TResult, IObservable<TResult>> _execute;
        private readonly Func<TParam, TResult, IObservable<TResult>> _discard;
        private readonly Func<IEnumerable<TParam>, (TParam, TResult)> _groupingAction;
        private readonly List<IHistoryEntryForGroup<TParam, TResult>> _groups = new();
        
        public bool IsEmpty => !_groups.Any();
        
        public GroupingByParam
        (
            Func<TParam, TResult, IObservable<TResult>> execute,
            Func<TParam, TResult, IObservable<TResult>> discard, 
            Func<IEnumerable<TParam>, (TParam, TResult)> groupingAction
        )
        {
            _execute = execute;
            _discard = discard;
            _groupingAction = groupingAction;
        }
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry)
        {
            _groups.Add(entry);
        }

        public IHistoryEntryForGroup<TParam, TResult> Group()
        {
            var parameters = _groups.Select(x => x.Param).ToList();
            var (param, result) = _groupingAction(parameters);
            
            return new HistoryEntryForGroup<TParam, TResult>
            (
                (entry) => { _discard(param, result);},
                (entry) => { _execute(param, result);},
                param, result
            );
        }

        public void Rollback()
        {
            var parameters = _groups.Select(x => x.Param).ToList();
            var (param, result) = _groupingAction(parameters);
            
            _discard(param, result);
        }
    }

}