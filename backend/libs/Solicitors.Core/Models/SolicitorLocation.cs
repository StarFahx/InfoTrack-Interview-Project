namespace Solicitors.Core.Models;

public record SolicitorLocation
{
    public Guid SolicitorLocationId { get; set; }
    
    public required string Address { get; set; }
    public required string Phone { get; set; }
    public DateTime LastModified { get; set; }

    public List<SolicitorLocationRating> LocationRatings { get; } = [];
    
    public Guid SolicitorId { get; set; }
    public required Solicitor Solicitor { get; set; }
}

public interface IRating
{
    decimal Value { get; }
    decimal Maximum { get; }
    string Provider { get; }
}