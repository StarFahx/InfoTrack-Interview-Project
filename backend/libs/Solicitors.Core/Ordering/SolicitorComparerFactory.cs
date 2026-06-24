using Solicitors.Core.Models;

namespace Solicitors.Core.Ordering;

internal class SolicitorComparerFactory : ISolicitorComparerFactory
{
    public IComparer<Solicitor> GetComparer(
        OrderingType? orderingType, 
        string ratingsProvider)
    {
        return orderingType switch
        {
            OrderingType.RatingAscending => new RatingComparer(ratingsProvider, true),
            OrderingType.RatingDescending => new RatingComparer(ratingsProvider, false),
            OrderingType.AlphabetAscending => new NameComparer(true),
            OrderingType.AlphabetDescending => new NameComparer(false),
            _ => DefaultSolicitorDataOrdering.Instance
        };
    }
}