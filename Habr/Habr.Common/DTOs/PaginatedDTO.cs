namespace Habr.Common.DTOs
{
    public class PaginatedDTO<T>
    {
        public IReadOnlyList<T> PagedList { get; set; }
        public int TotalPagesCount { get; set; }
        public int CurrentPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}