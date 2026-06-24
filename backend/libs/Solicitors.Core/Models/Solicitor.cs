namespace Solicitors.Core.Models;

public record Solicitor
{
    public Guid SolicitorId { get; set; }
    
    public required string Name { get; set; }
    public required string RelativeUrl { get; set; }
    
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? ShortDescription { get; set; }
    public DateTime LastModified { get; set; }

    public List<City> Cities { get; } = [];
    public List<SolicitorRating> Ratings { get; } = [];
    public List<SolicitorLocation> Locations { get; } = [];
}