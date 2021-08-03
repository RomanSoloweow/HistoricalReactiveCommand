using ReactiveUI.Fody.Helpers;

namespace HistoricalReactiveCommand.Sandbox.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public string ButtonText { get; set; } = "Test button";
    }
}
