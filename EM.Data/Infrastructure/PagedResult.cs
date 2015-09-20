using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.Data.Infrastructure
{
    public class PagedResult<T>
    {
        public PagedResult()
        {
        }

        public PagedResult(IList<T> list, int page, int pageSize, int totalCount)
        {
            Results = list;
            CurrentPage = page;
            PageSize = pageSize;
            RowCount = totalCount;
            Stats = new Dictionary<string, IList< string>>();
        }

        public IList<T> Results { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public Dictionary<string, IList<string>> Stats { get; set; }
        public int PageCount
        {
            get
            {
                int pageCount = (int)Math.Ceiling((double)RowCount / PageSize);
                if (pageCount > 0 && pageCount <= 100)
                {
                    return pageCount;
                }
                else if (pageCount > 100)//最大显示100页
                {
                    return 100;
                }
                else return 1;
            }
        }

        /// <summary>
        /// solr特供字段
        /// </summary>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }

        public IDictionary<string, JGroupedResults<T>> Grouping { get; set; }
    }


    public class JGroupedResults<T>
    {
        public JGroupedResults(){}

        public ICollection<JGroup<T>> Groups { get; set; }
        public int Matches { get; set; }
        public int? Ngroups { get; set; }
    }

    public class JGroup<T>
    {
        public JGroup() { }

        public ICollection<T> Documents { get; set; }
        public string GroupValue { get; set; }
        public int NumFound { get; set; }
    }

    public static class PageLinqExtensions
    {
        public static PagedResult<T> ToPagedList<T>
            (
                this IEnumerable<T> items,
                int pageIndex,
                int pageSize,
                int rowCount
            )
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var totalItemCount = rowCount;
            return new PagedResult<T>(items.ToList(), pageIndex, pageSize, totalItemCount);
        }
    }

}
