using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class ProductList
    {
        public int currentPage { get; set; }
        public int companyId { get; set; }
        public List<Product> results { get; set; }
        public int totalPages { get; set; }
        public int pageSize { get; set; }
        public string query { get; set; }
        public object sort { get; set; }
    }
}
