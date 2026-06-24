namespace Solicitors.Core;

public record PaginationResponse<T>(int Total, T[] Data)
{
    public T[] Data { get; set; } = Data;
    public int Total { get; set; } = Total;
}