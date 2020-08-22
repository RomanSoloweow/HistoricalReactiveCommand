using HistoricalReactiveCommand.Imports;
using System;
using System.Reactive;

namespace HistoricalReactiveCommand.History
{
    public interface IReactiveHistoryElement
    {
        object Parameter { get; set; }
        object Result { get; set; }
        string CommandKey { get; }
    }
}

