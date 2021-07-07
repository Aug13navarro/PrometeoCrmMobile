using Core.Model.Common;

namespace Core.Model
{
    public class SearchAssignmentRequest : PaginatedRequest
    {
        public int SaleTypeId { get; set; }
    }
}
