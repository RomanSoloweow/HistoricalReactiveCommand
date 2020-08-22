using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace HistoricalReactiveCommand.Sandbox.Helpers
{
    public static class BindingExtensions
    {
        public static IDisposable WhenViewModelAnyValue(this IViewFor view, Action<CompositeDisposable> block)
        {
            return view.WhenActivated(disposable =>
            {
                view.WhenAnyValue(x => x.ViewModel).Where(x => x != null).Subscribe(_ => block.Invoke(disposable)).DisposeWith(disposable);
            });
        }
    }
}
