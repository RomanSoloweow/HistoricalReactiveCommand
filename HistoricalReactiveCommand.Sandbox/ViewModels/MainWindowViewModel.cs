using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HistoricalReactiveCommand.Sandbox.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        [Reactive] public string ButtonText { get; set; } = "Test button";
    }
}
