using Core.Model.Common;

namespace Core.Model
{
    public class OrdersNotesPaginatedRequest : PaginatedRequest
    {
        public int userId { get; set; }
        public string query { get; set; }
    }
}
