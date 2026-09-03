using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Common
{
    public class PaginationQuery
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 5;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 5 : (value > MaxPageSize ? MaxPageSize : value);
        }

        public string? Search { get; set; }
    }
}
