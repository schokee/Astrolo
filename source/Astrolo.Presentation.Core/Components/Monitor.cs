using System.Reactive.Disposables;
using System.Reactive.Linq;
using Caliburn.Micro;

namespace Astrolo.Presentation.Core.Components
{
    public class Monitor<TState> : PropertyChangedBase
    {
        private readonly object _syncLock = new();

        private long _stackDepth;

        public bool IsActive => _stackDepth > 0;

        public bool IsIdle => !IsActive;

        public TState? CurrentState { get; private set; }

        public IDisposable Enter(TState newState)
        {
            TState? savedState;

            lock (_syncLock)
            {
                savedState = CurrentState;

                ++_stackDepth;
                CurrentState = newState;
            }

            Refresh();

            return Disposable.Create(() =>
            {
                lock (_syncLock)
                {
                    --_stackDepth;
                    CurrentState = savedState;
                }

                Refresh();
            });
        }

        public IObservable<T> Enter<T>(TState newState, Func<IObservable<T>> observableFactory)
        {
            return Observable.Using(() => Enter(newState), _ => observableFactory());
        }

        public async Task<T> EnterAsync<T>(TState newState, Func<T> select)
        {
            using (Enter(newState))
            {
                return await Task.Run(select);
            }
        }
    }
}
