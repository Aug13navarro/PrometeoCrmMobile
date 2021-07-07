namespace Core.Model.Common
{
    public class PaginatedRequest
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public PaginatedRequestSort Sort { get; set; }
    }
}
