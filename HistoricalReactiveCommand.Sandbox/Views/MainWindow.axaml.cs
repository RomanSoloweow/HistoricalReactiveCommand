using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using HistoricalReactiveCommand.Sandbox.Helpers;
using HistoricalReactiveCommand.Sandbox.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;
using HistoricalReactiveCommand.Sandbox.Helpers.Extensions;

namespace HistoricalReactiveCommand.Sandbox.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupBinding();
        }

        private void SetupBinding()
        {

            this.WhenViewModelAnyValue(disposable =>
            {
                this.OneWayBind(this.ViewModel, x => x.ButtonText, x => x.ButtonForTest.Content).DisposeWith(disposable);
            });
        }
    }
}
