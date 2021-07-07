using Core.Model.Common;

namespace Core.Model
{
    public class NotificationsPaginatedRequest : PaginatedRequest
    {
        public int GrdId { get; } = 3;
        public bool Viewed { get; set; }
    }
}
