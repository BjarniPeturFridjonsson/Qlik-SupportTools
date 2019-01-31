using System;

namespace Eir.Common
{
    public class Exchanger<T> : IDisposable where T : class
    {
        private readonly Func<T, T> _exchanger;
        private readonly T _temporaryInstance;
        private readonly T _previousInstance;

        public Exchanger(T temporaryInstance, Func<T, T> exchanger)
        {
            _temporaryInstance = temporaryInstance;
            _exchanger = exchanger;
            _previousInstance = _exchanger(temporaryInstance);
        }

        public void Dispose()
        {
            T temporaryInstance = _exchanger(_previousInstance);
            if (_temporaryInstance != temporaryInstance)
            {
                throw new Exception("Someone else has also exchanged the instance after me! This could potentially be a problem...");
            }
        }
    }
}