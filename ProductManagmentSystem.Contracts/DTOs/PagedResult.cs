using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagmentSystem.Contracts.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
