using System;
using System.Collections.Generic;
using System.Reactive;

namespace HistoricalReactiveCommand
{
    public class TransactionalEntry<TParam, TResult>  : IHistoryEntry<TParam, TResult> 
    {
        public TransactionalEntry(ITransaction<TParam, TResult> transaction)
        {
            
        }

        public Action Undo { get; }
        public Action Redo { get; }
        public TParam Param { get; }
        public TResult Result { get; }
    }
}