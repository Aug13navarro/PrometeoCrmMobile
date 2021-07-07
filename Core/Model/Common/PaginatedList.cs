using System;
using System.Collections.Generic;

namespace Core.Model.Common
{
    public class PaginatedList<T>
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public int ResultsCount { get; set; }
        public List<T> Results { get; set; }

        public PaginatedList(int pageSize, int currentPage, List<T> results, int totalCount)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            TotalCount = totalCount;
            ResultsCount = results.Count;
            Results = results;
        }
    }
}
