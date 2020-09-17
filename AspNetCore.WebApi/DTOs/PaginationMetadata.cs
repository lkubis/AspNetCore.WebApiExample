namespace AspNetCore.WebApi.DTOs
{
    public class PaginationMetadata
    {
        public long TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
