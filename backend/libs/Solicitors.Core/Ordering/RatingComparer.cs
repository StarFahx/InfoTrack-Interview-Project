using Solicitors.Core.Models;

namespace Solicitors.Core.Ordering;

internal class RatingComparer(string ratingsProvider, bool ascending) : IComparer<Solicitor>
{
    public int Compare(Solicitor? x, Solicitor? y)
    {
        var xRating = x?.Ratings.FirstOrDefault(r => r.Provider == ratingsProvider);
        var yRating = y?.Ratings.FirstOrDefault(r => r.Provider == ratingsProvider);

        decimal compare;
        
        if (xRating is null)
            compare = yRating is null ? 0 : 1;
        else if (yRating is null)
            compare = -1;
        else
        {
            compare = (xRating.Value / xRating.Maximum) - (yRating.Value / yRating.Maximum);

            if (!ascending)
                compare *= -1;
        }

        if (compare >= 0)
            return (int)Math.Ceiling(compare);
        else
            return (int)Math.Floor(compare);
        
    }
}