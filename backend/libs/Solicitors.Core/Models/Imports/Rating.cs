namespace Solicitors.Core.Models.Imports;

public record Rating(decimal Value, decimal MaxValue, string RatingProvider, string RatingProviderImgSrc)
{
    public decimal Value { get; } = Value;
    public decimal MaxValue { get; } = MaxValue;
    public string RatingProvider { get; } = RatingProvider;
    public string RatingProviderImgSrc { get; } = RatingProviderImgSrc;
}