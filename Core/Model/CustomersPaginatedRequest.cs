using Core.Model.Common;

namespace Core.Model
{
    public class CustomersPaginatedRequest : PaginatedRequest
    {
        public int UserId { get; set; }
        public string Query { get; set; }
    }
}
