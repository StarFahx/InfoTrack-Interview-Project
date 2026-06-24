using Solicitors.Core.Models.View;

namespace Solicitors.Core.Models;

public record SolicitorInfo
{
    internal SolicitorInfo(Solicitor solicitor)
    {
        Id = solicitor.SolicitorId;
        Name = solicitor.Name;
        ShortDescription = solicitor.ShortDescription;
        Phone = solicitor.Phone;
        Email = solicitor.Email;
        Website = solicitor.Website;

        Cities = solicitor.Cities
            .Select(city => city.Name)
            .ToArray();

        Ratings = solicitor.Ratings
            .Select(rating => new RatingInfo(rating))
            .ToArray();

        Locations = solicitor.Locations
            .Select(location => new LocationInfo(location))
            .ToArray();
    }

    public string Name { get; set; }
    public string? ShortDescription { get; set; }
    public Guid Id { get; set; }

    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    public string[] Cities { get; set; }
    public RatingInfo[] Ratings { get; set; }
    public LocationInfo[] Locations { get; set; }
}