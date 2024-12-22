﻿namespace SoundPlayer.Domain.BE
{
        public class PaginatedResponse<T>
        {
            public List<T> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }
}
