namespace HistoricalReactiveCommand.History
{
    public interface IReactiveHistoryElement<TParameter, TResult> : IReactiveCommandWithHistory
    {
        TParameter Parameter { get; set; }
        TResult Result { get; set; }
    }
}

