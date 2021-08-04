using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalReactiveCommand
{
    public class GroupingByParam<TParam, TResult>:IGrouping<TParam, TResult>
    {
        private Action<TParam> _execute;
        private Action<TParam> _discard;
        private Func<List<TParam>, TParam> _groupingAction;
        public GroupingByParam(
            Action<TParam> execute,
            Action<TParam> discard, 
            Func<List<TParam>, TParam> groupingAction)
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
            var parameters = Groups.Select(x => x.Param).ToList();
            var groupResult = _groupingAction(parameters);
            return new HistoryEntry(
                (entry) => { _discard(groupResult);},
                (entry) => { _execute(groupResult);});
        }

        public void Rollback()
        {
            var parameters = Groups.Select(x => x.Param).ToList();
            var groupResult = _groupingAction(parameters);
            _discard(groupResult);
        }
    }

}