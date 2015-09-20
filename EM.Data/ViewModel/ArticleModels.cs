using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topuc22Top.Model.Entities;

namespace Topuc22Top.Data.ViewModel
{
    public class ArticleModel
    {
        //public IList<RecommendedArticleResult> RecommendendArticles { get; set; }
        public IList<TB_Article> Articles { get; set; }
        public ArticleSiderModel SiderModel { get; set; }
    }

    public class ArticleSiderModel
    {
        public Dictionary<int, string> HotArticles { get; set; }
        public IList<Article_Tag> HotTags { get; set; }
        public IList<TB_Book> Books { get; set; }
    }

    public class ArticleTagModel
    {
        public string ArticleTag { get; set; }
        public int Count { get; set; }
        public IList<TB_Article> Articles { get; set; }
        public ArticleSiderModel SiderModel { get; set; }
    }

    public class ArticleDetailModel
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleContent { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Creator { get; set; }
        public string ArticleTags { get; set; }

        public Dictionary<int, string> ArticleTagItems { get; set; }
        public long Favor { get; set; }
        public ArticleSiderModel SiderModel { get; set; }
        public Dictionary<int, string> RelatedArticles { get; set; }

        //2014-8-7
        public int PreviousId { get; set; }
        public string PreviousTitle { get; set; }
        public int NextId { get; set; }
        public string NextTitle { get; set; }
    }
}
