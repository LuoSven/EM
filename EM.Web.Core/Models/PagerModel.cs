using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace EM.Web.Core
{
    public class PagerModel
    {
        public PagerModel(int recCount, int pageSize, int currentPage, string pagerClass = "", string anchor = "")
        {
            RecCount = recCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            PagerClass = pagerClass;
            Anchor = anchor;
        }
        public int RecCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        /// <summary>
        /// 最大页数
        /// </summary>
        public int PageCount
        {
            get
            {
                int pageCount = (int)Math.Ceiling((double)RecCount / PageSize);
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

        public string PagerClass { get; set; }
        public string Anchor { get; set; }

        public bool IsAjax { get; set; }

        public PagerAjaxOptions AjaxOptions { get; set; }
    }


}