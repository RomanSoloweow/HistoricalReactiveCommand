using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HistoricalReactiveCommand.Sandbox.ViewModels;
using HistoricalReactiveCommand.Sandbox.Views;

namespace HistoricalReactiveCommand.Sandbox
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            new MainWindow { DataContext = new MainWindowViewModel() }.Show();
            base.OnFrameworkInitializationCompleted();
        }
    }
}
