namespace SoundPlayer.Domain.Common
{
        public class PaginatedResponse<T> : BaseResponse
        {
            public IEnumerable<T> Items { get; set; }
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }

            public PaginatedResponse()
            {
                Items = new List<T>();
            }
        }
}
