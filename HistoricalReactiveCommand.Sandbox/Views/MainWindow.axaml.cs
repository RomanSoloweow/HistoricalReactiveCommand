using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using HistoricalReactiveCommand.Sandbox.Helpers;
using HistoricalReactiveCommand.Sandbox.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace HistoricalReactiveCommand.Sandbox.Views
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        Button ButtonForTest;
        public MainWindow()
        {
            InitializeComponent();
            SetupBinding();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            ButtonForTest = this.FindControl<Button>("ButtonForTest");
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
