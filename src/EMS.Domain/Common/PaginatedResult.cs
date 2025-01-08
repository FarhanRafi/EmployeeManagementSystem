namespace EMS.Domain.Common
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalRecords { get; set; }
        public int TotalFilteredRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
