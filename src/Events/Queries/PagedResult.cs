using System;
using System.Collections.Generic;
using System.Linq;

namespace Events
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public T[] Rows { get; set; }
        public int TotalRows { get; set; }
    }

    public class NextPagedResult<T>
    {
        public int LastId { get; set; }
        public int PageSize { get; set; }
        public T[] Rows { get; set; }
        public int TotalRows { get; set; }
    }

    public static class PagedResultExtension
    {
         public static PagedResult<T> GetPagedResult<T>(this IEnumerable<T> items, int pageNumber, int pageSize)
        {
            return new PagedResult<T>
            {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = items
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToArray(),
                TotalRows = items.Count()
            };
        }
    }
}