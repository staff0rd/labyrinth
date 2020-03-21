using MediatR;

namespace Events
{

    public class GetCollectionPagedByPageNumber<T> : GetCollection<Result<PagedResult<T>>>
    {
        public int PageNumber { get; set; }
    }

    public class GetCollectionPagedById<T> : GetCollection<Result<NextPagedResult<T>>>
    {
        public int LastId { get; set; }
    }

    public abstract class GetCollection<T> : IRequest<T>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Search { get; set; }
        public int PageSize { get; set; }
        public Network Network { get; set; }
    }
}