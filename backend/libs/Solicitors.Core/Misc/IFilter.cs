namespace Solicitors.Core.Misc;

public interface IFilter<in T>
{
    bool Filter(T item);
}

public class WrapperFilter<T>(
    IFilter<T>? inner,
    Func<T, bool> filter) : IFilter<T>
{
    public bool Filter(T item)
    {
        if (inner is not null)
            return inner.Filter(item) && filter(item);
        
        return filter(item);
    }
}