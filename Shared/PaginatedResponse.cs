namespace Shared
{
    public class PaginatedResponse<TData>
    {
        public PaginatedResponse(int pageIndex, int pageSize, int count, IEnumerable<TData> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;

        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IEnumerable<TData> Data { get; set; } = [];
        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling(Count / (double)PageSize)
            : 0;
    }
}
