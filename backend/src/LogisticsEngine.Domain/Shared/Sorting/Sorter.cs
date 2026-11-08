namespace LogisticsEngine.Domain.Shared.Sorting;

public sealed class Sorter<T> : ISorter<T>
{
    public IReadOnlyList<T> Sort<TKey>(IEnumerable<T> items, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(keySelector);
        return items.OrderBy(keySelector, comparer).ToArray();
    }
}
