using Solicitors.Core.Models;

namespace Solicitors.Core.Ordering;

internal class NameComparer(bool ascending) : IComparer<Solicitor>
{
    public int Compare(Solicitor? x, Solicitor? y)
    {
        int compare;
        if (ReferenceEquals(x, y)) 
            compare = 0;
        else if (y is null) 
            compare = 1;
        else if (x is null) 
            compare = -1;
        else
        {
            compare = string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);

            if (!ascending)
                compare *= -1;
        }

        return compare;
    }
}