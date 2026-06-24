namespace Solicitors.Core.Models.View;

public record LocationInfo
{
    public LocationInfo(SolicitorLocation location)
    {
        Address = location.Address;
        Phone = location.Phone;
        Ratings = location.LocationRatings
            .Select(rating => new RatingInfo(rating))
            .ToArray();
    }
    
    public string Address { get; }
    public string Phone { get; }

    public RatingInfo[] Ratings { get; }
}