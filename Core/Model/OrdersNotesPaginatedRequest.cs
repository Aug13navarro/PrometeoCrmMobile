using Core.Model.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class OrdersNotesPaginatedRequest : PaginatedRequest
    {
        public int userId { get; set; }
    }
}
