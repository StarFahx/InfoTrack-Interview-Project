namespace Solicitors.Core.Models.View;

public record RatingInfo
{
    public RatingInfo(IRating rating)
    {
        Value = rating.Value;
        Maximum = rating.Maximum;
        Provider = rating.Provider;
    }

    public decimal Value { get; }
    public decimal Maximum { get; }
    public string Provider { get; }
}