using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIntegrator.Common.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public IEnumerable<int> Pages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Pages = GetPages(TotalPages, pageIndex);
            this.AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        private IEnumerable<int> GetPages(int totalPages, int currentPage)
        {
            int startPage, endPage;
            if (totalPages <= 10)
            {
                // less than 10 total pages so show all
                startPage = 1;
                endPage = totalPages;
            }
            else
            {
                // more than 10 total pages so calculate start and end pages
                if (currentPage <= 6)
                {
                    startPage = 1;
                    endPage = 10;
                }
                else if (currentPage + 4 >= totalPages)
                {
                    startPage = totalPages - 9;
                    endPage = totalPages;
                }
                else
                {
                    startPage = currentPage - 5;
                    endPage = currentPage + 4;
                }
            }

            return Enumerable.Range(startPage, endPage - startPage + 1);
        }
    }
}
