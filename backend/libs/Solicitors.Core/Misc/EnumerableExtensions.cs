namespace Solicitors.Core.Misc;

public static class EnumerableExtensions
{
    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == batch.Capacity)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }
        
        if (batch.Count > 0)
            yield return batch.ToArray();
    }
}