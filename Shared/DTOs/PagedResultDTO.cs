namespace Shared.DTOs
{
    public class PagedResultDTO<T>
    {
        public IEnumerable<T> Items { get; init; } = new List<T>();

        public int Page { get; init; }
        public int PageSize { get; init; }

        public int TotalItems { get; init; }
        public int TotalPages { get; init; }

        public bool HasNext => Page < TotalPages;
        public bool HasPrevious => Page > 1;
    }
}
