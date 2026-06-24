namespace Solicitors.Core.Misc;

public readonly struct Pagination(uint currentPage, uint pageSize)
{
    public uint CurrentPage { get; } = currentPage;
    public uint PageSize { get; } = pageSize;
}