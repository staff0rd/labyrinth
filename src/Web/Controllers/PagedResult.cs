namespace Web
{
    public class PagedResult<T> {
        public T[] Rows { get; set;}

        public int PageSize { get; set;}

        public int Page { get; set;}

        public int TotalRows { get; set; }
    }
}