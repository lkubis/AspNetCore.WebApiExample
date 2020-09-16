using System.Collections.Generic;

namespace AspNetCore.Domain
{
    public sealed class PaginatedList<T> : List<T>
    {
        public long TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
