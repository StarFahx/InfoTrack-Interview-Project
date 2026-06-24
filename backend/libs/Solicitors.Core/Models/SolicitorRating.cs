namespace Solicitors.Core.Models;

public record SolicitorRating : IRating
{
    public Guid SolicitorRatingId { get; set; }
    
    public required decimal Value { get; set; }
    public required decimal Maximum { get; set; }
    public required string Provider { get; set; }
    public DateTime LastModified { get; set; }
    
    public Guid SolicitorId { get; set; }
    public required Solicitor Solicitor { get; set; }
}