using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Controllers
{
    public static class PagedResultExtension
    {
        public static PagedResult<TResult> GetPagedResult<T, TResult>(this IEnumerable<T> items, int pageNumber, int pageSize, Func<T, TResult> selector)
        {
            return new PagedResult<TResult>
            {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = items
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(selector)
                    .ToArray(),
                TotalRows = items.Count()
            };
        }
        
    }

    public class PagedResult<T> {
        public T[] Rows { get; set;}

        public int PageSize { get; set;}

        public int Page { get; set;}

        public int TotalRows { get; set; }
    }
}