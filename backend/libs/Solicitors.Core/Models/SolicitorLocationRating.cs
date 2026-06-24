namespace Solicitors.Core.Models;

public record SolicitorLocationRating : IRating
{
    public Guid SolicitorLocationRatingId { get; set; }
    
    public required decimal Value { get; set; }
    public required decimal Maximum { get; set; }
    public required string Provider { get; set; }
    public DateTime LastModified { get; set; }
    
    public Guid SolicitorLocationId { get; set; }
    public required SolicitorLocation SolicitorLocation { get; set; }
}