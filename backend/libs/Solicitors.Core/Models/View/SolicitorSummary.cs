namespace Solicitors.Core.Models.View;

public record SolicitorSummary
{
    internal SolicitorSummary(Solicitor solicitor, string ratingsProvider)
    {
        Id = solicitor.SolicitorId;
        Name = solicitor.Name;
        ShortDescription = solicitor.ShortDescription;
        var rating = solicitor.Ratings.FirstOrDefault(x => x.Provider == ratingsProvider);
        if (rating is not null)
            Rating = new RatingInfo(rating);
    }

    public Guid Id { get; }
    public string Name { get; }
    public string? ShortDescription { get; }
    public RatingInfo? Rating { get; }
}