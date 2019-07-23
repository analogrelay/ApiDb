using System;

namespace ApiDb
{
    public struct Disposable : IDisposable
    {
        private readonly Action _action;
        public Disposable(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));
        public void Dispose() => _action();

        public static Disposable Create(Action action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return new Disposable(action);
        }

        public static Disposable<T> Create<T>(Action<T> action, T state)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return new Disposable<T>(action, state);
        }
    }

    public struct Disposable<T> : IDisposable
    {
        private readonly Action<T> _action;
        private readonly T _state;
        public Disposable(Action<T> action, T state)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _state = state;
        }

        public void Dispose() => _action(_state);
    }
}
