using System;

namespace ApiDb
{
    public struct Disposable : IDisposable
    {
        private readonly Action _action;
        public Disposable(Action action) => _action = action;
        public void Dispose() => _action();

        public static Disposable Create(Action action) => new Disposable(action);
        public static Disposable<T> Create<T>(Action<T> action, T state) => new Disposable<T>(action, state);
    }

    public struct Disposable<T> : IDisposable
    {
        private readonly Action<T> _action;
        private readonly T _state;
        public Disposable(Action<T> action, T state)
        {
            _action = action;
            _state = state;
        }

        public void Dispose() => _action(_state);
    }
}
