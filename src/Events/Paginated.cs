using System.Collections.Generic;

namespace Events 
{
    public class Paginated<T>
    {
        public IEnumerable<T> Rows { get; set;}
        public int PageSize { get; set;}
    }
}