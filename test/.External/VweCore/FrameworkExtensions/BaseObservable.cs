using System.Collections.Generic;
using Light.GuardClauses;

namespace VweCore.FrameworkExtensions
{
    public abstract class BaseObservable<T> : IObservable<T>
        where T : class
    {
        protected BaseObservable(List<T>? observers = null) => Observers = observers ?? new List<T>();

        protected List<T> Observers { get; }

        public void AddObserver(T observer) => Observers.Add(observer.MustNotBeNull(nameof(observer)));

        public bool TryRemoveObserver(T observer)
        {
            for (var i = 0; i < Observers.Count; i++)
            {
                if (!ReferenceEquals(observer, Observers[i]))
                    continue;

                Observers.RemoveAt(i);
                return true;
            }

            return false;
        }
    }
}