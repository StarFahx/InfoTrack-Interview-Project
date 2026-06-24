using Solicitors.Core.Models;

namespace Solicitors.Core.Ordering;

public interface ISolicitorComparerFactory
{
    IComparer<Solicitor> GetComparer(
        OrderingType? orderingType,
        string ratingsProvider);
}