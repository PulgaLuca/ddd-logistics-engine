namespace LogisticsEngine.Domain.Shared.Sorting;

/// <summary>Ordina elementi usando la chiave selezionata dal chiamante.</summary>
public interface ISorter<T>
{
    IReadOnlyList<T> Sort<TKey>(IEnumerable<T> items, Func<T, TKey> keySelector, IComparer<TKey>? comparer = null);
}
