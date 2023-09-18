using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core
{
    public static class Reactive
    {
        public static IObservable<DeactivationEventArgs> ObserveDeactivation(this IDeactivate deactivate)
        {
            return Observable.Create<DeactivationEventArgs>(o =>
            {
                Task Handler(object _, DeactivationEventArgs args)
                {
                    o.OnNext(args);
                    return Task.CompletedTask;
                }

                deactivate.Deactivated += Handler;
                return Disposable.Create(() => deactivate.Deactivated -= Handler);
            });
        }

        public static IObservable<Unit> AsObservable(this CancellationToken cancellationToken)
        {
            return Observable.Create<Unit>(o => cancellationToken.Register(() =>
            {
                o.OnNext(Unit.Default);
                o.OnCompleted();
            }));
        }

        public static IObservable<T> TakeWhileNotCancelled<T>(this IObservable<T> source, CancellationToken cancellationToken)
        {
            return source.TakeWhile(_ => !cancellationToken.IsCancellationRequested);
        }


        public static IObservable<T> TakeUntilCancelled<T>(this IObservable<T> source, CancellationToken cancellationToken)
        {
            return source.TakeUntil(cancellationToken.AsObservable());
        }

        public static IObservable<T> CatchAndComplete<T>(this IObservable<T> source, Action<Exception> handleException)
        {
            return source.Catch((Exception exception) => { handleException(exception); return Observable.Empty<T>(); });
        }

        public static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, Func<int, Exception, bool> retryOnError, TimeSpan retryDelay)
        {
            return source.RetryWithDelay(retryOnError, _ => retryDelay);
        }

        public static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, Func<int, Exception, bool> retryOnError, Func<int, TimeSpan> getNextDelay)
        {
            return source.RetryWithDelay(retryOnError, getNextDelay, 0);
        }

        public static readonly Func<int, TimeSpan> ExponentialBackoff = n => TimeSpan.FromSeconds(Math.Pow(n, 2));

        private static IObservable<T> RetryWithDelay<T>(this IObservable<T> source, Func<int, Exception, bool> retryOnError, Func<int, TimeSpan> getNextDelay, int retryAttempt)
        {
            return Observable.Defer(() =>
            {
                var s = retryAttempt > 0
                    ? source.DelaySubscription(getNextDelay(retryAttempt))
                    : source;

                return s.Catch<T, Exception>(e => retryOnError(retryAttempt, e)
                    ? source.RetryWithDelay(retryOnError, getNextDelay, retryAttempt + 1)
                    : Observable.Throw<T>(e));
            });
        }

        public static IObservable<T> Observe<T>(this IEventAggregator eventAggregator)
        {
            return Observable.Create<T>(o =>
            {
                var handler = new Handler<T>(o);

                eventAggregator.SubscribeOnPublishedThread(handler);
                return Disposable.Create(() => eventAggregator.Unsubscribe(handler));
            });
        }

        private class Handler<T> : IHandle<T>
        {
            private readonly IObserver<T> _observer;

            public Handler(IObserver<T> observer)
            {
                _observer = observer;
            }

            public Task HandleAsync(T message, CancellationToken cancellationToken)
            {
                _observer.OnNext(message);
                return Task.CompletedTask;
            }
        }
    }
}
