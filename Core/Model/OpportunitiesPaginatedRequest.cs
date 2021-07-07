using Core.Model.Common;

namespace Core.Model
{
    public class OpportunitiesPaginatedRequest : PaginatedRequest
    {
        public string Query { get; set; }
    }
}
