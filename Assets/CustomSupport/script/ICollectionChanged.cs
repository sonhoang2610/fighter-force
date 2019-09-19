namespace EazyEngine.Tools
{
    /// <summary>
    /// IObservable.
    /// </summary>
    public interface ICollectionChanged
    {
        /// <summary>
        /// Occurs when changed collection (added item, removed item, replaced item).
        /// </summary>
        event OnChange OnCollectionChange;
    }
}