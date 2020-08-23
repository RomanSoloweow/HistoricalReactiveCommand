using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HistoricalReactiveCommand.History
{
    public sealed class  DefaultHistoriesManager:IHistoriesManager
    {
        private readonly IDictionary<string, IHistory> _histories = new ConcurrentDictionary<string, IHistory>();

        public void RegisterHistory(string historyKey, IHistory history)
        {
            if (_histories.ContainsKey(historyKey))
                throw new ArgumentException($"The key {historyKey} was already registered.", nameof(historyKey));
            _histories[historyKey] = history;
        }

        public IHistory ResolveHistory(string historyKey)
        {
            if (!_histories.ContainsKey(historyKey))
                throw new ArgumentException($"The key {historyKey} wasn't registered.", nameof(historyKey));
            return _histories[historyKey];
        }
    }
}
