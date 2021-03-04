namespace VweCore.FrameworkExtensions
{
    public interface IObservable<in T>
        where T : class
    {
        void AddObserver(T observer);

        bool TryRemoveObserver(T observer);
    }
}