using Solicitors.Core.Models;

namespace Solicitors.Core.Ordering;

internal class DefaultSolicitorDataOrdering : IComparer<Solicitor>
{
    public static IComparer<Solicitor> Instance { get; } = new DefaultSolicitorDataOrdering();

    private DefaultSolicitorDataOrdering()
    {
    }

    public int Compare(Solicitor? x, Solicitor? y)
    {
        if (x is null)
            return y is null ? 0 : int.MaxValue;
        
        else if (y is null)
            return int.MinValue;
        
        return Comparer<Guid>.Default.Compare(x.SolicitorId, y.SolicitorId);
    }
}