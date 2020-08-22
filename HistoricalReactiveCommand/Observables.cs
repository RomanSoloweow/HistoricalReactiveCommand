using System;
using System.Reactive;
using System.Reactive.Linq;

namespace HistoricalReactiveCommand
{
    /// <summary>
    /// Provides commonly required, statically-allocated, pre-canned observables.
    /// </summary>
    internal static class Observables
    {
        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>true</c>.
        /// </summary>
        public static readonly IObservable<bool> True = Observable.Return(true);

        /// <summary>
        /// An observable that ticks a single, Boolean value of <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;bool&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<bool> False = Observable.Return(false);

        /// <summary>
        /// An observable that ticks <c>Unit.Default</c> as a single value.</summary>
        /// <remarks>
        /// <para>
        /// This observable is equivalent to <c>Observable&lt;Unit&gt;.Default</c>, but is provided for convenience.
        /// </para>
        /// </remarks>
        public static readonly IObservable<Unit> Unit = Observable<Unit>.Default;
    }
}
